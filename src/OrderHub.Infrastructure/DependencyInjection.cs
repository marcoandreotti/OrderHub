using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderHub.Application.Abstractions.Persistence;
using OrderHub.Infrastructure.Persistence;
using OrderHub.Infrastructure.Persistence.Read;
using OrderHub.Infrastructure.Persistence.Write;
using OrderHub.Infrastructure.Persistence.Write.Repositories;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Application.Abstractions.Identity;
using OrderHub.Infrastructure.Identity;
using OrderHub.Application.Abstractions.Operations;
using OrderHub.Application.Abstractions.Catalog;
using OrderHub.Application.Abstractions.Customers;

namespace OrderHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection(DatabaseOptions.SectionName);
        services.AddOptions<DatabaseOptions>()
            .Bind(section)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Database connection string is required.")
            .ValidateOnStart();

        var connectionString = section[nameof(DatabaseOptions.ConnectionString)]
            ?? throw new InvalidOperationException("Database:ConnectionString is required.");

        services.AddDbContext<OrderHubDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly("OrderHub.Infrastructure.Migrations")));
        services.AddScoped<IReadConnectionFactory, NpgsqlReadConnectionFactory>();
        services.AddScoped<IEstablishmentRepository, EstablishmentRepository>();
        services.AddScoped<IEstablishmentReadGateway, EstablishmentReadGateway>();
        services.AddSingleton<IPasswordHasher, AspNetPasswordHasher>();
        services.AddScoped<IAdministrativeUserRepository, AdministrativeUserRepository>();
        services.AddScoped<IEstablishmentAccessGateway, EstablishmentAccessGateway>();
        services.AddScoped<IOperationsReadGateway, OperationsReadGateway>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryHierarchyGateway>(provider => (CategoryRepository)provider.GetRequiredService<ICategoryRepository>());
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAdditionalRepository, AdditionalRepository>();
        services.AddScoped<IAdditionalGroupRepository, AdditionalGroupRepository>();
        services.AddScoped<ICatalogReadGateway, CatalogReadGateway>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerReadGateway, CustomerReadGateway>();
        services.AddHealthChecks().AddDbContextCheck<OrderHubDbContext>("postgresql", tags: ["ready"]);

        return services;
    }
}
