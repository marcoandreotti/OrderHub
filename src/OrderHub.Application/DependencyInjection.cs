using Microsoft.Extensions.DependencyInjection;
using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.Dispatching;
using OrderHub.Application.Tenancy;
using OrderHub.Application.Tenancy.CreateEstablishment;
using FluentValidation;
using OrderHub.Application.Identity.CreateAdministrativeUser;
using OrderHub.Application.Abstractions.Catalog;
using OrderHub.Application.Catalog;
using OrderHub.Application.Abstractions.Customers;
using OrderHub.Application.Customers;

namespace OrderHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<EstablishmentScopeResolver>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ICommandHandler<CreateEstablishmentCommand, Guid>, CreateEstablishmentCommandHandler>();
        services.AddScoped<IValidator<CreateEstablishmentCommand>, CreateEstablishmentCommandValidator>();
        services.AddScoped<ICommandHandler<CreateAdministrativeUserCommand, Guid>, CreateAdministrativeUserCommandHandler>();
        services.AddScoped<IValidator<CreateAdministrativeUserCommand>, CreateAdministrativeUserCommandValidator>();
        services.AddScoped<ICommandHandler<UpsertCategoryCommand, Guid>, UpsertCategoryCommandHandler>();
        services.AddScoped<ICommandHandler<UpsertProductCommand, Guid>, UpsertProductCommandHandler>();
        services.AddScoped<ICommandHandler<UpsertAdditionalCommand, Guid>, UpsertAdditionalCommandHandler>();
        services.AddScoped<ICommandHandler<UpsertAdditionalGroupCommand, Guid>, UpsertAdditionalGroupCommandHandler>();
        services.AddScoped<IQueryHandler<GetAdministrativeCatalogQuery, CatalogReadModel>, GetAdministrativeCatalogQueryHandler>();
        services.AddScoped<IQueryHandler<GetPublicCatalogQuery, CatalogReadModel>, GetPublicCatalogQueryHandler>();
        services.AddScoped<IValidator<UpsertCategoryCommand>, UpsertCategoryCommandValidator>();
        services.AddScoped<IValidator<UpsertProductCommand>, UpsertProductCommandValidator>();
        services.AddScoped<IValidator<UpsertAdditionalCommand>, UpsertAdditionalCommandValidator>();
        services.AddScoped<IValidator<UpsertAdditionalGroupCommand>, UpsertAdditionalGroupCommandValidator>();
        services.AddScoped<IValidator<GetAdministrativeCatalogQuery>, GetAdministrativeCatalogQueryValidator>();
        services.AddScoped<IValidator<GetPublicCatalogQuery>, GetPublicCatalogQueryValidator>();
        services.AddScoped<ICommandHandler<UpsertCustomerCommand, Guid>, UpsertCustomerCommandHandler>();
        services.AddScoped<ICommandHandler<UpsertCustomerAddressCommand, Guid>, UpsertCustomerAddressCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveCustomerAddressCommand>, RemoveCustomerAddressCommandHandler>();
        services.AddScoped<IQueryHandler<SearchCustomersQuery, CustomerSearchResult>, SearchCustomersQueryHandler>();
        services.AddScoped<IValidator<UpsertCustomerCommand>, UpsertCustomerCommandValidator>();
        services.AddScoped<IValidator<UpsertCustomerAddressCommand>, UpsertCustomerAddressCommandValidator>();
        services.AddScoped<IValidator<RemoveCustomerAddressCommand>, RemoveCustomerAddressCommandValidator>();
        services.AddScoped<IValidator<SearchCustomersQuery>, SearchCustomersQueryValidator>();
        return services;
    }
}
