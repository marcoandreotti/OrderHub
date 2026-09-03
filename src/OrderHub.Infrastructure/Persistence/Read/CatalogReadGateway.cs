using Dapper;
using OrderHub.Application.Abstractions.Catalog;
using OrderHub.Application.Abstractions.Persistence;

namespace OrderHub.Infrastructure.Persistence.Read;

/// <summary>
/// Representa um gateway de leitura para o catálogo, fornecendo métodos para recuperar informações do catálogo, como estabelecimentos, categorias, produtos e grupos adicionais, a partir do banco de dados.
/// </summary>
public sealed class CatalogReadGateway(IReadConnectionFactory connectionFactory) : ICatalogReadGateway
{
    public Task<CatalogReadModel?> GetAdministrativeAsync(Guid tenantId, Guid establishmentId, CancellationToken cancellationToken) => LoadAsync("e.tenant_id = @TenantId and e.id = @EstablishmentId", new { TenantId = tenantId, EstablishmentId = establishmentId }, false, cancellationToken);

    public Task<CatalogReadModel?> GetPublicAsync(string normalizedSlug, CancellationToken cancellationToken) => LoadAsync("e.slug = @Slug and e.is_active = true and t.is_active = true", new { Slug = normalizedSlug }, true, cancellationToken);

    private async Task<CatalogReadModel?> LoadAsync(string establishmentFilter, object parameters, bool publicOnly, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        var establishment = await connection.QuerySingleOrDefaultAsync<EstablishmentRow>(new CommandDefinition($"select e.id Id, e.tenant_id TenantId, e.trade_name Name, e.slug Slug from tenancy.establishment e join tenancy.tenant t on t.id=e.tenant_id where {establishmentFilter}", parameters, cancellationToken: cancellationToken));
        if (establishment is null) return null;
        var scope = new { establishment.TenantId, EstablishmentId = establishment.Id };
        var active = publicOnly ? " and is_active = true" : string.Empty;
        var categories = (await connection.QueryAsync<CategoryRow>(new CommandDefinition($"select id Id, parent_category_id ParentId, name Name, description Description, \"order\" \"Order\", image_url ImageUrl, is_active IsActive from catalog.category where tenant_id=@TenantId and establishment_id=@EstablishmentId{active} order by \"order\", name", scope, cancellationToken: cancellationToken))).ToList();
        var products = (await connection.QueryAsync<ProductRow>(new CommandDefinition($"select id Id, category_id CategoryId, code Code, name Name, description Description, base_price BasePrice, is_featured IsFeatured, is_active IsActive, allows_notes AllowsNotes from catalog.product where tenant_id=@TenantId and establishment_id=@EstablishmentId{active} order by name", scope, cancellationToken: cancellationToken))).ToList();
        var productIds = products.Select(x => x.Id).ToArray();
        var images = productIds.Length == 0 ? [] : (await connection.QueryAsync<ImageRow>(new CommandDefinition("select id Id, product_id ProductId, url Url, \"order\" \"Order\", is_principal IsPrincipal from catalog.product_image where product_id=any(@Ids) order by \"order\"", new { Ids = productIds }, cancellationToken: cancellationToken))).ToList();
        var variations = productIds.Length == 0 ? [] : (await connection.QueryAsync<VariationRow>(new CommandDefinition($"select id Id, product_id ProductId, name Name, price Price, \"order\" \"Order\", is_active IsActive from catalog.product_variation where product_id=any(@Ids){active} order by \"order\"", new { Ids = productIds }, cancellationToken: cancellationToken))).ToList();
        var groups = (await connection.QueryAsync<GroupRow>(new CommandDefinition($"select g.id Id, g.name Name, g.minimum_selection MinimumSelection, g.maximum_selection MaximumSelection, g.is_active IsActive from catalog.additional_group g where g.tenant_id=@TenantId and g.establishment_id=@EstablishmentId{(publicOnly ? " and g.is_active = true" : string.Empty)}", scope, cancellationToken: cancellationToken))).ToList();
        var groupItems = (await connection.QueryAsync<GroupItemRow>(new CommandDefinition($"select i.group_id GroupId, i.additional_id Id, a.name Name, a.price Price, a.is_active IsActive, i.\"order\" \"Order\" from catalog.additional_group_item i join catalog.additional a on a.tenant_id=i.tenant_id and a.establishment_id=i.establishment_id and a.id=i.additional_id where i.tenant_id=@TenantId and i.establishment_id=@EstablishmentId{(publicOnly ? " and a.is_active = true" : string.Empty)} order by i.\"order\"", scope, cancellationToken: cancellationToken))).ToList();
        var links = productIds.Length == 0 ? [] : (await connection.QueryAsync<ProductGroupRow>(new CommandDefinition("select product_id ProductId, group_id GroupId, \"order\" \"Order\" from catalog.product_additional_group where tenant_id=@TenantId and establishment_id=@EstablishmentId and product_id=any(@Ids) order by \"order\"", new { establishment.TenantId, EstablishmentId = establishment.Id, Ids = productIds }, cancellationToken: cancellationToken))).ToList();
        var groupById = groups.ToDictionary(x => x.Id, x => new AdditionalGroupReadModel(x.Id, x.Name, x.MinimumSelection, x.MaximumSelection, x.IsActive, 0, groupItems.Where(i => i.GroupId == x.Id).Select(i => new AdditionalReadModel(i.Id, i.Name, i.Price, i.IsActive, i.Order)).ToList()));
        var productModels = products.Select(p => new ProductReadModel(p.Id, p.Code, p.Name, p.Description, p.BasePrice, p.IsFeatured, p.IsActive, p.AllowsNotes, images.Where(x => x.ProductId == p.Id).Select(x => new ProductImageReadModel(x.Id, x.Url, x.Order, x.IsPrincipal)).ToList(), variations.Where(x => x.ProductId == p.Id).Select(x => new ProductVariationReadModel(x.Id, x.Name, x.Price, x.Order, x.IsActive)).ToList(), links.Where(x => x.ProductId == p.Id && groupById.ContainsKey(x.GroupId)).Select(x => groupById[x.GroupId] with { Order = x.Order }).ToList())).ToList();
        var categoryModels = categories.Select(c => new CategoryReadModel(c.Id, c.ParentId, c.Name, c.Description, c.Order, c.ImageUrl, c.IsActive, productModels.Where(p => products.Single(x => x.Id == p.Id).CategoryId == c.Id).ToList())).Where(c => !publicOnly || c.Products.Count > 0).ToList();
        return new CatalogReadModel(establishment.Id, establishment.Name, establishment.Slug, categoryModels);
    }

    private sealed record EstablishmentRow(Guid Id, Guid TenantId, string Name, string Slug);
    private sealed record CategoryRow(Guid Id, Guid? ParentId, string Name, string? Description, int Order, string? ImageUrl, bool IsActive);
    private sealed record ProductRow(Guid Id, Guid CategoryId, string Code, string Name, string? Description, decimal BasePrice, bool IsFeatured, bool IsActive, bool AllowsNotes);
    private sealed record ImageRow(Guid Id, Guid ProductId, string Url, int Order, bool IsPrincipal);
    private sealed record VariationRow(Guid Id, Guid ProductId, string Name, decimal Price, int Order, bool IsActive);
    private sealed record GroupRow(Guid Id, string Name, int MinimumSelection, int MaximumSelection, bool IsActive);
    private sealed record GroupItemRow(Guid GroupId, Guid Id, string Name, decimal Price, bool IsActive, int Order);
    private sealed record ProductGroupRow(Guid ProductId, Guid GroupId, int Order);
}