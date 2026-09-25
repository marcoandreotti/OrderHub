using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Identity;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Application.Identity.Authentication;
using OrderHub.Application.Platform;
using OrderHub.Infrastructure.Identity;
using OrderHub.Infrastructure.Persistence.Read;
using OrderHub.Infrastructure.Persistence.Write;

namespace OrderHub.Infrastructure;

/// <summary>
/// Fornece métodos de extensão para registrar serviços relacionados ao provisionamento da plataforma no contêiner de injeção de dependência.
/// </summary>
public static class PlatformProvisioningDependencyInjection
{
    public static IServiceCollection AddPlatformProvisioning(this IServiceCollection services)
    {
        services.AddScoped<IPlatformProvisioningRepository, PlatformProvisioningRepository>();
        services.AddScoped<IPlatformProvisioningTransaction, PlatformProvisioningTransaction>();
        services.AddScoped<IPlatformProvisioningReadGateway, PlatformProvisioningReadGateway>();
        services.Replace(ServiceDescriptor.Scoped<IAuthenticationSessionResolver, ProvisioningAuthenticationSessionResolver>());
        services.Replace(ServiceDescriptor.Scoped<ICommandHandler<CompleteAuthenticationCommand, AuthenticationTokens>, ProvisioningCompleteAuthenticationCommandHandler>());
        services.Replace(ServiceDescriptor.Scoped<ICommandHandler<RefreshAuthenticationCommand, AuthenticationTokens>, ProvisioningRefreshAuthenticationCommandHandler>());
        services.Replace(ServiceDescriptor.Scoped<ICommandHandler<ChangeTemporaryPasswordCommand>, ProvisioningChangeTemporaryPasswordCommandHandler>());
        services.AddScoped<ICommandHandler<ProvisionTenantCommand, PlatformProvisioningResult>, ProvisionTenantCommandHandler>();
        services.AddScoped<ICommandHandler<AddPlatformEstablishmentCommand, PlatformProvisioningResult>, AddPlatformEstablishmentCommandHandler>();
        services.AddScoped<IQueryHandler<SearchPlatformTenantsQuery, PlatformTenantSearchResult>, SearchPlatformTenantsQueryHandler>();
        services.AddScoped<IQueryHandler<GetPlatformTenantQuery, PlatformTenantReadModel>, GetPlatformTenantQueryHandler>();
        services.AddScoped<IQueryHandler<GetPlatformProvisioningQuery, PlatformProvisioningResult>, GetPlatformProvisioningQueryHandler>();
        services.AddScoped<IValidator<ProvisionTenantCommand>, ProvisionTenantCommandValidator>();
        services.AddScoped<IValidator<AddPlatformEstablishmentCommand>, AddPlatformEstablishmentCommandValidator>();
        services.AddScoped<IValidator<SearchPlatformTenantsQuery>, SearchPlatformTenantsQueryValidator>();
        services.AddScoped<IValidator<GetPlatformTenantQuery>, GetPlatformTenantQueryValidator>();
        services.AddScoped<IValidator<GetPlatformProvisioningQuery>, GetPlatformProvisioningQueryValidator>();
        return services;
    }
}