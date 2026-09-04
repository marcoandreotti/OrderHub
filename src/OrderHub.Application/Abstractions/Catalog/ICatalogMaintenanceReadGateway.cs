using OrderHub.Application.Catalog;

namespace OrderHub.Application.Abstractions.Catalog;

/// <summary>Consulta recursos reutilizáveis sem exigir associação prévia com produtos.</summary>
public interface ICatalogMaintenanceReadGateway
{
    Task<AdditionalSearchResult> SearchAdditionalsAsync(Guid tenantId, SearchAdditionalsQuery query, CancellationToken cancellationToken);
    Task<AdditionalGroupSearchResult> SearchGroupsAsync(Guid tenantId, SearchAdditionalGroupsQuery query, CancellationToken cancellationToken);
}

public sealed record AdditionalSearchResult(int Total, IReadOnlyList<AdditionalReadModel> Items);
public sealed record AdditionalGroupSearchResult(int Total, IReadOnlyList<AdditionalGroupReadModel> Items);
