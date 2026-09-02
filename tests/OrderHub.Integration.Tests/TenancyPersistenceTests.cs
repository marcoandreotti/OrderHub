using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderHub.Domain.Tenancy;
using OrderHub.Infrastructure.Persistence;
using OrderHub.Infrastructure.Persistence.Read;
using OrderHub.Infrastructure.Persistence.Write;
using OrderHub.Infrastructure.Persistence.Write.Repositories;
using Testcontainers.PostgreSql;

namespace OrderHub.Integration.Tests;

public sealed class TenancyPersistenceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public Task InitializeAsync() => database.StartAsync();
    public async Task DisposeAsync() => await database.DisposeAsync();

    [Fact]
    public async Task Ef_write_and_dapper_read_enforce_tenant_and_active_public_scope()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>()
            .UseNpgsql(database.GetConnectionString())
            .Options;
        await using var context = new OrderHubDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var now = DateTimeOffset.UtcNow;
        var tenantA = Tenant.Create("Tenant A", now);
        var tenantB = Tenant.Create("Tenant B", now);
        context.Tenants.AddRange(tenantA, tenantB);
        await context.SaveChangesAsync();

        var repository = new EstablishmentRepository(context);
        var unitA = Establishment.Create(tenantA.Id, "Unit A", new Slug("unit-a"), now);
        var unitB = Establishment.Create(tenantB.Id, "Unit B", new Slug("unit-b"), now);
        await repository.AddAsync(unitA, CancellationToken.None);
        await repository.AddAsync(unitB, CancellationToken.None);

        var gateway = new EstablishmentReadGateway(new NpgsqlReadConnectionFactory(Options.Create(new DatabaseOptions
        {
            ConnectionString = database.GetConnectionString()
        })));

        Assert.NotNull(await gateway.FindAsync(tenantA.Id, unitA.Id, CancellationToken.None));
        Assert.Null(await gateway.FindAsync(tenantA.Id, unitB.Id, CancellationToken.None));
        Assert.Equal(unitA.Id, (await gateway.ResolvePublicSlugAsync("unit-a", CancellationToken.None))?.Id);

        unitA.Deactivate(now.AddMinutes(1));
        await context.SaveChangesAsync();
        Assert.Null(await gateway.ResolvePublicSlugAsync("unit-a", CancellationToken.None));
    }
}
