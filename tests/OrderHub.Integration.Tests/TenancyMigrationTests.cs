using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using OrderHub.Infrastructure.Identity;
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

    [Fact]
    public async Task Order_lifecycle_migration_upgrades_rolls_back_and_reapplies()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString(), npgsql => npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName)).Options;
        await using var context = new OrderHubDbContext(options); var migrator = context.Database.GetService<IMigrator>();
        await context.Database.MigrateAsync(); await using var connection = new NpgsqlConnection(database.GetConnectionString()); await connection.OpenAsync();
        var expected = new[] { "order", "order_item", "order_item_additional", "order_number_counter", "order_status_history", "public_order_request" };
        Assert.Equal(expected, (await connection.QueryAsync<string>("select table_name from information_schema.tables where table_schema='orders' order by table_name;")).ToArray());
        await migrator.MigrateAsync("20260902201903_CustomerRecords");
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='orders';"));
        await context.Database.MigrateAsync();
        Assert.Equal(expected.Length, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='orders';"));
    }

    [Fact]
    public async Task Coupon_management_migration_upgrades_rolls_back_and_reapplies()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString(), npgsql => npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName)).Options;
        await using var context = new OrderHubDbContext(options); var migrator = context.Database.GetService<IMigrator>(); await context.Database.MigrateAsync();
        await using var connection = new NpgsqlConnection(database.GetConnectionString()); await connection.OpenAsync();
        Assert.Equal(["coupon", "coupon_use"], (await connection.QueryAsync<string>("select table_name from information_schema.tables where table_schema='promotions' order by table_name;")).ToArray());
        await migrator.MigrateAsync("20260902220405_OrderLifecycle"); Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='promotions';"));
        await context.Database.MigrateAsync(); Assert.Equal(2, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='promotions';"));
    }

    [Fact]
    public async Task Order_payments_migration_upgrades_rolls_back_and_reapplies()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString(), npgsql => npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName)).Options;
        await using var context = new OrderHubDbContext(options); var migrator = context.Database.GetService<IMigrator>(); await context.Database.MigrateAsync(); await using var connection = new NpgsqlConnection(database.GetConnectionString()); await connection.OpenAsync();
        Assert.Equal(["payment", "payment_idempotency", "payment_method"], (await connection.QueryAsync<string>("select table_name from information_schema.tables where table_schema='payments' order by table_name;")).ToArray());
        await migrator.MigrateAsync("20260902231354_CouponManagement"); Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='payments';")); await context.Database.MigrateAsync(); Assert.Equal(3, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='payments';"));
    }

    [Fact]
    public async Task Public_ordering_migration_upgrades_rolls_back_and_reapplies()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString(), npgsql => npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName)).Options;
        await using var context = new OrderHubDbContext(options); var migrator = context.Database.GetService<IMigrator>(); await context.Database.MigrateAsync(); await using var connection = new NpgsqlConnection(database.GetConnectionString()); await connection.OpenAsync();
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='orders' and table_name='public_order_request';"));
        await migrator.MigrateAsync("20260902235922_OrderPayments");
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='orders' and table_name='public_order_request';"));
        await context.Database.MigrateAsync();
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='orders' and table_name='public_order_request';"));
    }

    [Fact]
    public async Task Administrative_authentication_migration_upgrades_rolls_back_and_reapplies()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql(database.GetConnectionString(), npgsql => npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName)).Options;
        await using var context = new OrderHubDbContext(options);
        var migrator = context.Database.GetService<IMigrator>();
        await context.Database.MigrateAsync();
        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();
        var expected = new[] { "administrative_session", "authentication_challenge", "platform_user" };
        Assert.Equal(expected, (await connection.QueryAsync<string>("select table_name from information_schema.tables where table_schema='identity' and table_name in ('administrative_session','authentication_challenge','platform_user') order by table_name;")).ToArray());
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.columns where table_schema='tenancy' and table_name='tenant' and column_name='public_code';"));
        await migrator.MigrateAsync("20260903001324_PublicOrderingApi");
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='identity' and table_name in ('administrative_session','authentication_challenge','platform_user');"));
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.columns where table_schema='tenancy' and table_name='tenant' and column_name='public_code';"));
        await context.Database.MigrateAsync();
        Assert.Equal(expected.Length, await connection.ExecuteScalarAsync<int>("select count(*) from information_schema.tables where table_schema='identity' and table_name in ('administrative_session','authentication_challenge','platform_user');"));
    }

    [Fact]
    public async Task Concurrent_platform_bootstrap_creates_exactly_one_superuser()
    {
        var contextOptions = new DbContextOptionsBuilder<OrderHubDbContext>()
            .UseNpgsql(database.GetConnectionString(), npgsql =>
                npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName))
            .Options;
        await using (var migrationContext = new OrderHubDbContext(contextOptions))
        {
            await migrationContext.Database.MigrateAsync();
        }

        var bootstrapOptions = Options.Create(new PlatformBootstrapOptions
        {
            Email = "first.superuser@orderhub.test",
            TemporaryPassword = "temporary-password"
        });

        async Task BootstrapAsync()
        {
            await using var context = new OrderHubDbContext(contextOptions);
            var bootstrapper = new PlatformBootstrapper(
                new AuthenticationRepository(context),
                new AspNetPasswordHasher(),
                bootstrapOptions,
                TimeProvider.System);
            await bootstrapper.InitializeAsync(CancellationToken.None);
        }

        await Task.WhenAll(BootstrapAsync(), BootstrapAsync());

        await using var verificationContext = new OrderHubDbContext(contextOptions);
        var user = Assert.Single(await verificationContext.PlatformUsers.AsNoTracking().ToListAsync());
        Assert.True(user.IsActive);
        Assert.True(user.PasswordChangeRequired);
        Assert.NotEqual("temporary-password", user.PasswordHash);

        await BootstrapAsync();
        Assert.Equal(1, await verificationContext.PlatformUsers.CountAsync());
    }
}
