using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OrderHub.Infrastructure.Migrations;
using OrderHub.Infrastructure.Persistence.Write;
using Npgsql;
using Testcontainers.PostgreSql;

namespace OrderHub.Integration.Tests;

public sealed class TenancyMigrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public Task InitializeAsync() => database.StartAsync();
    public async Task DisposeAsync() => await database.DisposeAsync();

    [Fact]
    public async Task Initial_tenancy_migration_applies_and_rolls_back_on_empty_database()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>()
            .UseNpgsql(database.GetConnectionString(), npgsql =>
                npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName))
            .Options;
        await using var context = new OrderHubDbContext(options);

        await context.Database.MigrateAsync();
        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();
        var tablesAfterUp = await connection.QueryAsync<string>(
            "select table_name from information_schema.tables where table_schema = 'tenancy';");
        Assert.Equal(
            ["establishment", "establishment_theme", "tenant"],
            tablesAfterUp.Order(StringComparer.Ordinal).ToArray());
        Assert.Equal(6, await connection.ExecuteScalarAsync<int>("select count(*) from identity.administrative_role;"));
        Assert.Equal(2, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='operations';"));

        var migrator = context.Database.GetService<IMigrator>();
        await migrator.MigrateAsync(Migration.InitialDatabase);
        var tablesAfterDown = await connection.QueryAsync<string>(
            "select table_name from information_schema.tables where table_schema = 'tenancy';");
        Assert.Empty(tablesAfterDown);
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema in ('identity','operations');"));
    }

    [Fact]
    public async Task Product_catalog_migration_upgrades_rolls_back_and_reapplies()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString(), npgsql => npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName)).Options;
        await using var context = new OrderHubDbContext(options); var migrator = context.Database.GetService<IMigrator>();
        await context.Database.MigrateAsync(); await using var connection = new NpgsqlConnection(database.GetConnectionString()); await connection.OpenAsync();
        var expected = new[] { "additional", "additional_group", "additional_group_item", "category", "product", "product_additional_group", "product_image", "product_variation" };
        Assert.Equal(expected, (await connection.QueryAsync<string>("select table_name from information_schema.tables where table_schema='catalog' order by table_name;")).ToArray());
        await migrator.MigrateAsync("20260820120132_IdentityOperations");
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='catalog';"));
        await context.Database.MigrateAsync();
        Assert.Equal(expected.Length, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='catalog';"));
    }

    [Fact]
    public async Task Customer_records_migration_upgrades_rolls_back_and_reapplies()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString(), npgsql => npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName)).Options;
        await using var context = new OrderHubDbContext(options);
        var migrator = context.Database.GetService<IMigrator>();
        await context.Database.MigrateAsync();
        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();

        Assert.Equal(["customer", "customer_address"], (await connection.QueryAsync<string>("select table_name from information_schema.tables where table_schema='customers' order by table_name;")).ToArray());
        await migrator.MigrateAsync("20260820135644_ProductCatalog");
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='customers';"));
        await context.Database.MigrateAsync();
        Assert.Equal(2, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='customers';"));
    }
}
