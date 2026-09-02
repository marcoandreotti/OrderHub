using OrderHub.Application.Abstractions.Catalog;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.Exceptions;
using OrderHub.Application.Tenancy;
using OrderHub.Domain.Tenancy;

namespace OrderHub.Application.Catalog;

public sealed class GetAdministrativeCatalogQueryHandler(EstablishmentScopeResolver scopeResolver, ICatalogReadGateway gateway) : IQueryHandler<GetAdministrativeCatalogQuery, CatalogReadModel>
{
    public async Task<CatalogReadModel> HandleAsync(GetAdministrativeCatalogQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeResolver.ResolveAsync(query.EstablishmentId, cancellationToken);
        return await gateway.GetAdministrativeAsync(scope.TenantId, scope.EstablishmentId, cancellationToken) ?? throw new NotFoundException("Catalog was not found.");
    }
}

public sealed class GetPublicCatalogQueryHandler(ICatalogReadGateway gateway) : IQueryHandler<GetPublicCatalogQuery, CatalogReadModel>
{
    public async Task<CatalogReadModel> HandleAsync(GetPublicCatalogQuery query, CancellationToken cancellationToken) =>
        await gateway.GetPublicAsync(new Slug(query.Slug).Value, cancellationToken) ?? throw new NotFoundException("Catalog was not found.");
}
