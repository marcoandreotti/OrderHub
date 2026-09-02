using OrderHub.Application.Abstractions.Catalog;
using OrderHub.Application.Abstractions.Queries;

namespace OrderHub.Application.Catalog;

public sealed record GetAdministrativeCatalogQuery(Guid EstablishmentId) : IQuery<CatalogReadModel>;
public sealed record GetPublicCatalogQuery(string Slug) : IQuery<CatalogReadModel>;
