using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderHub.Domain.Ordering;
using OrderHub.Domain.SharedKernel;
using OrderHub.Domain.Tenancy;
using OrderHub.Infrastructure.Persistence;
using OrderHub.Infrastructure.Persistence.Read;
using OrderHub.Infrastructure.Persistence.Write;
using Testcontainers.PostgreSql;

namespace OrderHub.Integration.Tests;

public sealed class OrderPersistenceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17-alpine").Build();
    public Task InitializeAsync() => database.StartAsync();
    public async Task DisposeAsync() => await database.DisposeAsync();

    [Fact]
    public async Task Ef_and_Dapper_preserve_order_snapshots_totals_and_history()
    {
        var options = CreateOptions();
        Guid tenantId; Guid establishmentId; Guid orderId;
        await using (var context = new OrderHubDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
            var now = DateTimeOffset.UtcNow; var tenant = Tenant.Create("Group", now); var unit = Establishment.Create(tenant.Id, "Unit", new Slug("unit"), now);
            var order = Order.Create(tenant.Id, unit.Id, OrderServiceType.Delivery, null, "Maria", "11999998888", null, new("Rua A", "10", null, "Centro", "São Paulo", "SP", "01001000"), now);
            order.AddItem(Guid.NewGuid(), null, "Produto antigo", null, new Money(12.50m), new Quantity(2), [new(Guid.NewGuid(), "Adicional antigo", new Money(1.25m), new Quantity(1))], "observação", now);
            order.Confirm(1, now); order.StartPreparation(now.AddMinutes(1), Guid.NewGuid());
            context.AddRange(tenant, unit, order); await context.SaveChangesAsync();
            tenantId = tenant.Id; establishmentId = unit.Id; orderId = order.Id;
        }

        var gateway = new OrderReadGateway(new NpgsqlReadConnectionFactory(Microsoft.Extensions.Options.Options.Create(new DatabaseOptions { ConnectionString = database.GetConnectionString() })));
        var result = await gateway.GetAsync(tenantId, establishmentId, orderId, CancellationToken.None);

        Assert.NotNull(result); Assert.Equal("Produto antigo", Assert.Single(result.Items).ProductName); Assert.Equal("Adicional antigo", Assert.Single(result.Items[0].Additionals).Name);
        Assert.Equal(27.50m, result.Total); Assert.Equal(2, result.History.Count); Assert.Equal("Rua A", result.DeliveryAddress!.Street);
        Assert.Null(await gateway.GetAsync(Guid.NewGuid(), establishmentId, orderId, CancellationToken.None));
        var filtered=await gateway.SearchAsync(tenantId,establishmentId,null,null,OrderStatus.Preparing,1,OrderServiceType.Delivery,1,20,CancellationToken.None);
        Assert.Equal(1,filtered.Total);Assert.Equal(orderId,Assert.Single(filtered.Items).Id);
        Assert.Empty((await gateway.SearchAsync(Guid.NewGuid(),establishmentId,null,null,null,null,null,1,20,CancellationToken.None)).Items);
        Assert.Empty((await gateway.SearchAsync(tenantId,establishmentId,null,null,OrderStatus.Completed,null,null,1,20,CancellationToken.None)).Items);
    }

    [Fact]
    public async Task Concurrent_sequence_reservations_are_distinct_and_monotonic_per_establishment()
    {
        var options = CreateOptions(); Guid tenantId; Guid establishmentId;
        await using (var setup = new OrderHubDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync(); var now = DateTimeOffset.UtcNow; var tenant = Tenant.Create("Group", now); var unit = Establishment.Create(tenant.Id, "Unit", new Slug("unit"), now); setup.AddRange(tenant, unit); await setup.SaveChangesAsync(); tenantId = tenant.Id; establishmentId = unit.Id;
        }

        var reservations = Enumerable.Range(0, 8).Select(async _ =>
        {
            await using var context = new OrderHubDbContext(options); await using var transaction = await context.Database.BeginTransactionAsync();
            var number = await new OrderNumberSequence(context).ReserveAsync(tenantId, establishmentId, CancellationToken.None); await transaction.CommitAsync(); return number;
        });
        var numbers = await Task.WhenAll(reservations);

        Assert.Equal(Enumerable.Range(1, 8).Select(x => (long)x), numbers.Order());
    }

    private DbContextOptions<OrderHubDbContext> CreateOptions() => new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString()).Options;
}
