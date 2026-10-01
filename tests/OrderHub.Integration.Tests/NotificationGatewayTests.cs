using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderHub.Application.Abstractions.Communications;
using OrderHub.Application.Abstractions.Persistence;
using OrderHub.Application.Tenancy;
using OrderHub.Domain.Tenancy;
using OrderHub.Infrastructure;
using OrderHub.Infrastructure.Persistence.Write;
using Testcontainers.PostgreSql;

namespace OrderHub.Integration.Tests;

public sealed class NotificationGatewayTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17-alpine").Build();
    public Task InitializeAsync() => database.StartAsync();
    public async Task DisposeAsync() => await database.DisposeAsync();

    [Theory]
    [InlineData(NotificationChannel.Email, "customer@example.test")]
    [InlineData(NotificationChannel.WhatsApp, "+15551234567")]
    public async Task Outbox_delivers_each_enabled_channel_and_records_attempt(NotificationChannel channel, string destination)
    {
        var sender = new FakeNotificationSender(channel, new NotificationProviderResult(NotificationProviderOutcome.Accepted, "provider-123", null));
        var services = CreateServices(sender);
        await using var provider = services.BuildServiceProvider();
        var (scope, notificationId) = await CreateRequestAsync(provider, channel, destination, requiresConsent: false);

        var worker = provider.GetServices<IHostedService>().Single(item => item.GetType().Name == "OutboxProcessingWorker");
        await worker.StartAsync(CancellationToken.None);
        try
        {
            await WaitForStatusAsync(provider, scope, notificationId, NotificationDeliveryStatus.AcceptedByProvider);
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }

        Assert.Equal(1, sender.Calls);
        await using var readScope = provider.CreateAsyncScope();
        var reads = readScope.ServiceProvider.GetRequiredService<INotificationReadGateway>();
        var attempts = await reads.ListAttemptsAsync(scope, notificationId, CancellationToken.None);
        var attempt = Assert.Single(attempts);
        Assert.Equal(NotificationDeliveryStatus.AcceptedByProvider, attempt.Status);
        Assert.Equal("provider-123", attempt.ProviderMessageId);
    }

    [Fact]
    public async Task Missing_required_consent_blocks_delivery_without_calling_provider()
    {
        var sender = new FakeNotificationSender(NotificationChannel.Email, new NotificationProviderResult(NotificationProviderOutcome.Accepted, "unexpected", null));
        var services = CreateServices(sender);
        await using var provider = services.BuildServiceProvider();
        var (scope, notificationId) = await CreateRequestAsync(provider, NotificationChannel.Email, "customer@example.test", requiresConsent: true);
        await using (var consentScope = provider.CreateAsyncScope())
        {
            var repository = consentScope.ServiceProvider.GetRequiredService<INotificationWriteRepository>();
            var now = DateTimeOffset.UtcNow;
            await repository.SetConsentAsync(scope,
                new(NotificationChannel.Email, "order.update", "customer@example.test", true, now, "verified checkout opt-in"),
                now, CancellationToken.None);
            await repository.SetConsentAsync(scope,
                new(NotificationChannel.Email, "order.update", "customer@example.test", false, now.AddSeconds(1), "customer opt-out"),
                now.AddSeconds(1), CancellationToken.None);
        }

        var worker = provider.GetServices<IHostedService>().Single(item => item.GetType().Name == "OutboxProcessingWorker");
        await worker.StartAsync(CancellationToken.None);
        try
        {
            await WaitForStatusAsync(provider, scope, notificationId, NotificationDeliveryStatus.BlockedByConsent);
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }

        Assert.Equal(0, sender.Calls);
        await using var readScope = provider.CreateAsyncScope();
        var reads = readScope.ServiceProvider.GetRequiredService<INotificationReadGateway>();
        var attempt = Assert.Single(await reads.ListAttemptsAsync(scope, notificationId, CancellationToken.None));
        Assert.Equal("consent_required", attempt.SafeErrorCode);
        var consentHistory = await reads.ListConsentsAsync(scope, CancellationToken.None);
        Assert.Equal(new[] { false, true }, consentHistory.Select(entry => entry.IsGranted));
    }

    [Fact]
    public async Task Transient_provider_failure_schedules_retry_and_keeps_each_attempt()
    {
        var sender = new FakeNotificationSender(NotificationChannel.Email,
            new NotificationProviderResult(NotificationProviderOutcome.RetryableFailure, null, "smtp_421"),
            new NotificationProviderResult(NotificationProviderOutcome.Accepted, "provider-after-retry", null));
        var services = CreateServices(sender, retryDelaySeconds: 1);
        await using var provider = services.BuildServiceProvider();
        var (scope, notificationId) = await CreateRequestAsync(provider, NotificationChannel.Email, "customer@example.test", requiresConsent: false);
        var worker = provider.GetServices<IHostedService>().Single(item => item.GetType().Name == "OutboxProcessingWorker");
        await worker.StartAsync(CancellationToken.None);
        try
        {
            await WaitForStatusAsync(provider, scope, notificationId, NotificationDeliveryStatus.AcceptedByProvider);
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }

        Assert.Equal(2, sender.Calls);
        await using var readScope = provider.CreateAsyncScope();
        var reads = readScope.ServiceProvider.GetRequiredService<INotificationReadGateway>();
        var attempts = await reads.ListAttemptsAsync(scope, notificationId, CancellationToken.None);
        Assert.Equal(new[] { NotificationDeliveryStatus.RetryScheduled, NotificationDeliveryStatus.AcceptedByProvider },
            attempts.Select(attempt => attempt.Status));
    }

    [Fact]
    public async Task Templates_and_history_are_scoped_to_the_authorized_tenant_and_establishment()
    {
        var services = CreateServices(new FakeNotificationSender(NotificationChannel.Email,
            new NotificationProviderResult(NotificationProviderOutcome.Accepted, "provider", null)));
        await using var provider = services.BuildServiceProvider();
        await using var child = provider.CreateAsyncScope();
        var context = child.ServiceProvider.GetRequiredService<OrderHubDbContext>();
        await context.Database.MigrateAsync();
        var now = DateTimeOffset.UtcNow;
        var tenantA = Tenant.Create("Communications A", now);
        var tenantB = Tenant.Create("Communications B", now);
        var establishmentA = Establishment.Create(tenantA.Id, "A", new Slug("communications-a-" + Guid.NewGuid().ToString("N")), now);
        var establishmentB = Establishment.Create(tenantB.Id, "B", new Slug("communications-b-" + Guid.NewGuid().ToString("N")), now);
        context.AddRange(tenantA, tenantB, establishmentA, establishmentB);
        await context.SaveChangesAsync();
        var scopeA = new OperationalScope(tenantA.Id, Guid.NewGuid(), establishmentA.Id);
        var scopeB = new OperationalScope(tenantB.Id, Guid.NewGuid(), establishmentB.Id);
        var repository = child.ServiceProvider.GetRequiredService<INotificationWriteRepository>();
        var reads = child.ServiceProvider.GetRequiredService<INotificationReadGateway>();
        var templateId = await repository.UpsertTemplateAsync(scopeA,
            new(null, "order.update", NotificationChannel.Email, "pt_BR", "Order", "Order {{id}}", null, false, true), now, CancellationToken.None);
        var template = await repository.FindTemplateAsync(scopeA, templateId, CancellationToken.None);
        Assert.NotNull(template);
        _ = await repository.RequestAsync(scopeA,
            new(establishmentA.Id, templateId, "customer@example.test", new Dictionary<string, string> { ["id"] = "12" }, "tenant-a-request"),
            template!, "{\"id\":\"12\"}", now, CancellationToken.None);

        Assert.Null(await repository.FindTemplateAsync(scopeB, templateId, CancellationToken.None));
        await Assert.ThrowsAsync<OrderHub.Application.Exceptions.NotFoundException>(() => repository.UpsertTemplateAsync(scopeB,
            new(templateId, "order.update", NotificationChannel.Email, "pt_BR", "Changed", "Changed", null, false, true),
            now, CancellationToken.None));
        Assert.Empty(await reads.ListTemplatesAsync(scopeB, CancellationToken.None));
        Assert.Empty(await reads.ListHistoryAsync(scopeB, 1, 10, CancellationToken.None));
        Assert.Single(await reads.ListTemplatesAsync(scopeA, CancellationToken.None));
        Assert.Single(await reads.ListHistoryAsync(scopeA, 1, 10, CancellationToken.None));
    }

    private ServiceCollection CreateServices(FakeNotificationSender sender, int retryDelaySeconds = 30)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Database:ConnectionString"] = database.GetConnectionString(),
            ["Outbox:PollingIntervalSeconds"] = "1",
            ["Outbox:CleanupIntervalHours"] = "24",
            ["NotificationDelivery:MaximumAttempts"] = "2",
            ["NotificationDelivery:InitialRetryDelaySeconds"] = retryDelaySeconds.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["NotificationDelivery:MaximumRetryDelaySeconds"] = retryDelaySeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        services.AddSingleton(TimeProvider.System);
        services.AddInfrastructure(configuration);
        services.RemoveAll<INotificationChannelSender>();
        services.AddSingleton<INotificationChannelSender>(sender);
        return services;
    }

    private static async Task<(OperationalScope Scope, Guid NotificationId)> CreateRequestAsync(
        ServiceProvider provider, NotificationChannel channel, string destination, bool requiresConsent)
    {
        await using var serviceScope = provider.CreateAsyncScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<OrderHubDbContext>();
        await context.Database.MigrateAsync();
        var now = DateTimeOffset.UtcNow;
        var tenant = Tenant.Create("Notification tenant", now);
        var establishment = Establishment.Create(tenant.Id, "Notification unit", new Slug("notify-" + Guid.NewGuid().ToString("N")), now);
        context.AddRange(tenant, establishment);
        await context.SaveChangesAsync();
        var scope = new OperationalScope(tenant.Id, Guid.NewGuid(), establishment.Id);
        var repository = serviceScope.ServiceProvider.GetRequiredService<INotificationWriteRepository>();
        var templateId = await repository.UpsertTemplateAsync(scope,
            new NotificationTemplateDraft(null, "order.update", channel, "pt_BR", channel == NotificationChannel.Email ? "Pedido atualizado" : "",
                "Pedido {{1}} atualizado", channel == NotificationChannel.WhatsApp ? "order_update" : null,
                requiresConsent, true), now, CancellationToken.None);
        var template = await repository.FindTemplateAsync(scope, templateId, CancellationToken.None);
        Assert.NotNull(template);
        var first = await repository.RequestAsync(scope, new(establishment.Id, templateId, destination,
            new Dictionary<string, string> { ["1"] = "42" }, "request-42"), template!, "{\"1\":\"42\"}", now, CancellationToken.None);
        var replay = await repository.RequestAsync(scope, new(establishment.Id, templateId, destination,
            new Dictionary<string, string> { ["1"] = "42" }, "request-42"), template!, "{\"1\":\"42\"}", now, CancellationToken.None);
        Assert.True(first.Created);
        Assert.False(replay.Created);
        Assert.Equal(first.Id, replay.Id);
        return (scope, first.Id);
    }

    private static async Task WaitForStatusAsync(IServiceProvider provider, OperationalScope scope, Guid notificationId,
        NotificationDeliveryStatus expected)
    {
        var until = DateTimeOffset.UtcNow.AddSeconds(20);
        while (DateTimeOffset.UtcNow < until)
        {
            await using var child = provider.CreateAsyncScope();
            var connection = child.ServiceProvider.GetRequiredService<OrderHubDbContext>().Database.GetDbConnection();
            var state = await connection.QuerySingleOrDefaultAsync<string>(
                "select status from communications.notification where tenant_id = @TenantId and establishment_id = @EstablishmentId and id = @Id",
                new { scope.TenantId, scope.EstablishmentId, Id = notificationId });
            if (Enum.TryParse<NotificationDeliveryStatus>(state, out var parsed) && parsed == expected) return;
            await Task.Delay(100);
        }
        Assert.Fail($"Notification did not reach status {expected}.");
    }

    private sealed class FakeNotificationSender(NotificationChannel channel, params NotificationProviderResult[] results) : INotificationChannelSender
    {
        public NotificationChannel Channel => channel;
        public int Calls { get; private set; }
        public Task<NotificationProviderResult> SendAsync(NotificationChannelSendRequest request, CancellationToken cancellationToken)
        {
            Assert.Equal(channel, request.Channel);
            var result = results[Math.Min(Calls, results.Length - 1)];
            Calls++;
            return Task.FromResult(result);
        }
    }
}
