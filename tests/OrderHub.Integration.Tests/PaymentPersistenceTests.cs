using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Application.Exceptions;
using OrderHub.Application.Payments;
using OrderHub.Application.Tenancy;
using OrderHub.Domain.Ordering;
using OrderHub.Domain.Payments;
using OrderHub.Domain.SharedKernel;
using OrderHub.Domain.Tenancy;
using OrderHub.Infrastructure.Persistence;
using OrderHub.Infrastructure.Persistence.Read;
using OrderHub.Infrastructure.Persistence.Write;
using OrderHub.Infrastructure.Persistence.Write.Repositories;
using Testcontainers.PostgreSql;

namespace OrderHub.Integration.Tests;

public sealed class PaymentPersistenceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17-alpine").Build();
    public Task InitializeAsync() => database.StartAsync(); public async Task DisposeAsync() => await database.DisposeAsync();

    [Fact]
    public async Task Concurrent_confirmations_are_serialized_and_idempotent_against_authoritative_total()
    {
        var options = CreateOptions(); var now = DateTimeOffset.UtcNow; Guid tenantId; Guid unitId; Guid firstId; Guid secondId; Guid orderId;
        await using (var setup = new OrderHubDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync(); var tenant = Tenant.Create("Group", now); var unit = Establishment.Create(tenant.Id, "Unit", new Slug("unit"), now); var order = Order.Create(tenant.Id, unit.Id, OrderServiceType.Pickup, null, null, null, null, null, now); order.AddItem(Guid.NewGuid(), null, "Produto", null, new Money(50), new Quantity(1), [], null, now); order.Confirm(1, now); var method = PaymentMethod.Create(tenant.Id, unit.Id, "PIX", "Pix", true, false, now); var first = Payment.Create(tenant.Id, unit.Id, order.Id, method, new Money(30), null, now); var second = Payment.Create(tenant.Id, unit.Id, order.Id, method, new Money(30), null, now); setup.AddRange(tenant, unit, order, method, first, second); await setup.SaveChangesAsync(); tenantId=tenant.Id; unitId=unit.Id; orderId=order.Id; firstId=first.Id; secondId=second.Id;
        }
        async Task<bool> Confirm(Guid paymentId, string key)
        {
            await using var context = new OrderHubDbContext(options); var handler = Handler(context, tenantId, unitId);
            try { await handler.HandleAsync(new(unitId, paymentId, 30, key, null), CancellationToken.None); return true; } catch (ConflictException) { return false; }
        }
        var results = await Task.WhenAll(Confirm(firstId, "payment-operation-1"), Confirm(secondId, "payment-operation-2")); Assert.Single(results, x => x);
        await using (var replayContext = new OrderHubDbContext(options))
        {
            var confirmedId = await replayContext.Payments.Where(x => x.Status == PaymentStatus.Confirmed).Select(x => x.Id).SingleAsync(); var key = confirmedId == firstId ? "payment-operation-1" : "payment-operation-2"; var handler = Handler(replayContext, tenantId, unitId); Assert.Equal(confirmedId, await handler.HandleAsync(new(unitId, confirmedId, 30, key, null), CancellationToken.None));
        }
        var gateway = new PaymentReadGateway(new NpgsqlReadConnectionFactory(Options.Create(new DatabaseOptions { ConnectionString = database.GetConnectionString() }))); var read = await gateway.GetOrderPaymentsAsync(tenantId, unitId, orderId, CancellationToken.None);
        Assert.Equal(30m, read.ConfirmedAmount); Assert.False(read.IsFullyCovered); Assert.Equal("Confirmed", read.OperationalStatus); Assert.Equal(2, read.Payments.Count);
        var methods=await gateway.SearchMethodsAsync(tenantId,unitId,"pix",true,1,20,CancellationToken.None);Assert.Equal(1,methods.Total);Assert.Equal("PIX",Assert.Single(methods.Items).Code);
        Assert.Empty((await gateway.SearchMethodsAsync(Guid.NewGuid(),unitId,null,null,1,20,CancellationToken.None)).Items);
    }

    private ConfirmPaymentCommandHandler Handler(OrderHubDbContext context, Guid tenantId, Guid unitId) => new(new EstablishmentScopeResolver(new TenantContext(tenantId), new Access(unitId)), new PaymentRepository(context), new PaymentIdempotencyRepository(context), new PaymentOrderGateway(context), new PaymentConfirmationTransaction(context), TimeProvider.System);
    private DbContextOptions<OrderHubDbContext> CreateOptions() => new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString()).Options;
    private sealed class TenantContext(Guid tenantId) : ITenantContext { public bool HasTenant => true; public Guid TenantId => tenantId; public bool HasUser => true; public Guid UserId => Guid.NewGuid(); public Guid GetRequiredTenantId()=>TenantId; public Guid GetRequiredUserId()=>UserId; }
    private sealed class Access(Guid unitId) : IEstablishmentAccessGateway { public Task<bool> HasActiveAccessAsync(Guid tenantId, Guid userId, Guid establishmentId, CancellationToken cancellationToken)=>Task.FromResult(establishmentId==unitId); }
}
