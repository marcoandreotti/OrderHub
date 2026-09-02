using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Customers;
using OrderHub.Domain.Tenancy;
using OrderHub.Infrastructure.Persistence;
using OrderHub.Infrastructure.Persistence.Read;
using OrderHub.Infrastructure.Persistence.Write;
using OrderHub.Infrastructure.Persistence.Write.Repositories;
using Testcontainers.PostgreSql;

namespace OrderHub.Integration.Tests;

public sealed class CustomerPersistenceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public Task InitializeAsync() => database.StartAsync();
    public async Task DisposeAsync() => await database.DisposeAsync();

    [Fact]
    public async Task Ef_and_Dapper_keep_customers_and_addresses_isolated_by_establishment()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString()).Options;
        await using var context = new OrderHubDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var now = DateTimeOffset.UtcNow;
        var tenant = Tenant.Create("Group", now);
        var firstUnit = Establishment.Create(tenant.Id, "First", new Slug("first"), now);
        var secondUnit = Establishment.Create(tenant.Id, "Second", new Slug("second"), now);
        context.AddRange(tenant, firstUnit, secondUnit);
        await context.SaveChangesAsync();

        var first = Customer.Create(tenant.Id, firstUnit.Id, "Maria First", "11999998888", "maria@first.test", now);
        first.AddAddress("Casa", "Rua A", "10", null, "Centro", "São Paulo", "SP", "01001000", true, now);
        var second = Customer.Create(tenant.Id, secondUnit.Id, "Maria Second", "11999998888", "maria@second.test", now);
        second.AddAddress("Casa", "Rua B", "20", null, "Centro", "São Paulo", "SP", "01002000", true, now);
        context.Customers.AddRange(first, second);
        await context.SaveChangesAsync();

        var gateway = CreateGateway();
        var result = await gateway.SearchAsync(tenant.Id, firstUnit.Id, "11999998888", 1, 20, CancellationToken.None);

        var customer = Assert.Single(result.Items);
        Assert.Equal(first.Id, customer.Id);
        Assert.Equal("Rua A", Assert.Single(customer.Addresses).Street);
        Assert.DoesNotContain(result.Items, item => item.Id == second.Id);
    }

    [Fact]
    public async Task Database_allows_same_phone_in_other_unit_and_rejects_duplicate_in_same_unit()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString()).Options;
        await using var context = new OrderHubDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var now = DateTimeOffset.UtcNow;
        var tenant = Tenant.Create("Group", now);
        var firstUnit = Establishment.Create(tenant.Id, "First", new Slug("first"), now);
        var secondUnit = Establishment.Create(tenant.Id, "Second", new Slug("second"), now);
        context.AddRange(tenant, firstUnit, secondUnit);
        await context.SaveChangesAsync();
        context.Customers.Add(Customer.Create(tenant.Id, firstUnit.Id, "First", "11999998888", null, now));
        context.Customers.Add(Customer.Create(tenant.Id, secondUnit.Id, "Second", "11999998888", null, now));
        await context.SaveChangesAsync();

        var repository = new CustomerRepository(context);
        await Assert.ThrowsAsync<ConflictException>(() => repository.AddAsync(
            Customer.Create(tenant.Id, firstUnit.Id, "Duplicate", "(11) 99999-8888", null, now),
            CancellationToken.None));
    }

    [Fact]
    public async Task Concurrent_primary_address_changes_keep_one_atomic_winner()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString()).Options;
        Guid customerId;
        Guid firstAddressId;
        Guid secondAddressId;
        var now = DateTimeOffset.UtcNow;

        await using (var setup = new OrderHubDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            var tenant = Tenant.Create("Group", now);
            var unit = Establishment.Create(tenant.Id, "Unit", new Slug("unit"), now);
            var customer = Customer.Create(tenant.Id, unit.Id, "Maria", "11999998888", null, now);
            firstAddressId = customer.AddAddress("Casa", "Rua A", "10", null, "Centro", "São Paulo", "SP", "01001000", true, now).Id;
            secondAddressId = customer.AddAddress("Trabalho", "Rua B", "20", null, "Centro", "São Paulo", "SP", "01002000", false, now).Id;
            customerId = customer.Id;
            setup.AddRange(tenant, unit, customer);
            await setup.SaveChangesAsync();
        }

        await using var firstContext = new OrderHubDbContext(options);
        await using var secondContext = new OrderHubDbContext(options);
        var firstRepository = new CustomerRepository(firstContext);
        var secondRepository = new CustomerRepository(secondContext);
        var firstCustomer = await firstContext.Customers.Include(x => x.Addresses).SingleAsync(x => x.Id == customerId);
        var secondCustomer = await secondContext.Customers.Include(x => x.Addresses).SingleAsync(x => x.Id == customerId);
        firstCustomer.UpdateAddress(secondAddressId, "Trabalho", "Rua B", "20", null, "Centro", "São Paulo", "SP", "01002000", true, now.AddMinutes(1));
        secondCustomer.UpdateAddress(firstAddressId, "Casa", "Rua A", "10", null, "Centro", "São Paulo", "SP", "01001000", true, now.AddMinutes(2));

        await firstRepository.SaveChangesAsync(CancellationToken.None);
        await Assert.ThrowsAsync<ConflictException>(() => secondRepository.SaveChangesAsync(CancellationToken.None));

        await using var verification = new OrderHubDbContext(options);
        var persisted = await verification.Customers.Include(x => x.Addresses).SingleAsync(x => x.Id == customerId);
        Assert.Equal(secondAddressId, Assert.Single(persisted.Addresses, address => address.IsPrimary).Id);
    }

    private CustomerReadGateway CreateGateway() =>
        new(new NpgsqlReadConnectionFactory(Options.Create(new DatabaseOptions { ConnectionString = database.GetConnectionString() })));
}
