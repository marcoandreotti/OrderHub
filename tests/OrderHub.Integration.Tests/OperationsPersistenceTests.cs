using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderHub.Domain.Operations;
using OrderHub.Domain.Tenancy;
using OrderHub.Infrastructure.Persistence;
using OrderHub.Infrastructure.Persistence.Read;
using OrderHub.Infrastructure.Persistence.Write;
using Testcontainers.PostgreSql;

namespace OrderHub.Integration.Tests;

public sealed class OperationsPersistenceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17-alpine").Build();
    public Task InitializeAsync() => database.StartAsync();
    public async Task DisposeAsync() => await database.DisposeAsync();

    [Fact]
    public async Task Qr_requires_matching_slug_and_token_and_hours_are_unit_scoped()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString()).Options;
        await using var context = new OrderHubDbContext(options); await context.Database.EnsureCreatedAsync();
        var now = DateTimeOffset.UtcNow; var tenant = Tenant.Create("Tenant", now);
        var first = Establishment.Create(tenant.Id, "First", new Slug("ops-first"), now);
        var second = Establishment.Create(tenant.Id, "Second", new Slug("ops-second"), now);
        var table = ServiceTable.Create(tenant.Id, first.Id, "A1");
        var hours = BusinessHours.Create(tenant.Id, first.Id, DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(20, 0));
        context.AddRange(tenant, first, second, table, hours); await context.SaveChangesAsync();
        var gateway = new OperationsReadGateway(new NpgsqlReadConnectionFactory(Options.Create(new DatabaseOptions { ConnectionString = database.GetConnectionString() })));
        Assert.NotNull(await gateway.ResolveTableAsync("ops-first", table.QrCodeToken, CancellationToken.None));
        Assert.Null(await gateway.ResolveTableAsync("ops-second", table.QrCodeToken, CancellationToken.None));
        Assert.True(await gateway.IsOpenAsync(tenant.Id, first.Id, DayOfWeek.Monday, new TimeOnly(12, 0), CancellationToken.None));
        Assert.False(await gateway.IsOpenAsync(tenant.Id, second.Id, DayOfWeek.Monday, new TimeOnly(12, 0), CancellationToken.None));
    }
}
