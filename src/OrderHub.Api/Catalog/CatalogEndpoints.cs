using OrderHub.Application.Abstractions.Catalog;
using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.Catalog;
using OrderHub.Application.Identity;
using OrderHub.Contracts.Catalog;

namespace OrderHub.Api.Catalog;

internal static class CatalogEndpoints
{
    /// <summary>Registra os endpoints administrativos e públicos do catálogo.</summary>
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var admin = endpoints.MapGroup("/api/admin/establishments/{establishmentId:guid}/catalog").RequireAuthorization(AdministrativePolicies.Management);
        admin.MapGet("/", GetAdministrativeAsync);
        admin.MapPost("/categories", (Guid establishmentId, UpsertCategoryRequest request, ICommandDispatcher dispatcher, CancellationToken ct) => UpsertCategoryAsync(establishmentId, null, request, dispatcher, ct));
        admin.MapPut("/categories/{id:guid}", (Guid establishmentId, Guid id, UpsertCategoryRequest request, ICommandDispatcher dispatcher, CancellationToken ct) => UpsertCategoryAsync(establishmentId, id, request, dispatcher, ct));
        admin.MapPost("/products", (Guid establishmentId, UpsertProductRequest request, ICommandDispatcher dispatcher, CancellationToken ct) => UpsertProductAsync(establishmentId, null, request, dispatcher, ct));
        admin.MapPut("/products/{id:guid}", (Guid establishmentId, Guid id, UpsertProductRequest request, ICommandDispatcher dispatcher, CancellationToken ct) => UpsertProductAsync(establishmentId, id, request, dispatcher, ct));
        admin.MapPost("/additionals", (Guid establishmentId, UpsertAdditionalRequest request, ICommandDispatcher dispatcher, CancellationToken ct) => UpsertAdditionalAsync(establishmentId, null, request, dispatcher, ct));
        admin.MapPut("/additionals/{id:guid}", (Guid establishmentId, Guid id, UpsertAdditionalRequest request, ICommandDispatcher dispatcher, CancellationToken ct) => UpsertAdditionalAsync(establishmentId, id, request, dispatcher, ct));
        admin.MapPost("/additional-groups", (Guid establishmentId, UpsertAdditionalGroupRequest request, ICommandDispatcher dispatcher, CancellationToken ct) => UpsertGroupAsync(establishmentId, null, request, dispatcher, ct));
        admin.MapPut("/additional-groups/{id:guid}", (Guid establishmentId, Guid id, UpsertAdditionalGroupRequest request, ICommandDispatcher dispatcher, CancellationToken ct) => UpsertGroupAsync(establishmentId, id, request, dispatcher, ct));
        endpoints.MapGet("/api/public/establishments/{slug}/catalog", GetPublicAsync).AllowAnonymous();
        return endpoints;
    }

    private static async Task<IResult> GetAdministrativeAsync(Guid establishmentId, IQueryDispatcher dispatcher, CancellationToken ct) => Results.Ok(Map(await dispatcher.DispatchAsync<GetAdministrativeCatalogQuery, CatalogReadModel>(new(establishmentId), ct)));
    private static async Task<IResult> GetPublicAsync(string slug, IQueryDispatcher dispatcher, CancellationToken ct) => Results.Ok(Map(await dispatcher.DispatchAsync<GetPublicCatalogQuery, CatalogReadModel>(new(slug), ct)));
    private static async Task<IResult> UpsertCategoryAsync(Guid establishmentId, Guid? id, UpsertCategoryRequest r, ICommandDispatcher d, CancellationToken ct) => Results.Ok(new { id = await d.DispatchAsync<UpsertCategoryCommand, Guid>(new(establishmentId, id, r.Name, r.Description, r.Order, r.ImageUrl, r.ParentId, r.IsActive), ct) });
    private static async Task<IResult> UpsertProductAsync(Guid establishmentId, Guid? id, UpsertProductRequest r, ICommandDispatcher d, CancellationToken ct) => Results.Ok(new { id = await d.DispatchAsync<UpsertProductCommand, Guid>(new(establishmentId, id, r.CategoryId, r.Code, r.Name, r.Description, r.BasePrice, r.IsFeatured, r.IsActive, r.AllowsNotes, r.Images.Select(x => new ProductImageInput(x.Url, x.Order, x.IsPrincipal)).ToList(), r.Variations.Select(x => new ProductVariationInput(x.Name, x.Price, x.Order, x.IsActive)).ToList(), r.AdditionalGroups.Select(x => new ProductGroupInput(x.GroupId, x.Order)).ToList()), ct) });
    private static async Task<IResult> UpsertAdditionalAsync(Guid establishmentId, Guid? id, UpsertAdditionalRequest r, ICommandDispatcher d, CancellationToken ct) => Results.Ok(new { id = await d.DispatchAsync<UpsertAdditionalCommand, Guid>(new(establishmentId, id, r.Name, r.Price, r.IsActive), ct) });
    private static async Task<IResult> UpsertGroupAsync(Guid establishmentId, Guid? id, UpsertAdditionalGroupRequest r, ICommandDispatcher d, CancellationToken ct) => Results.Ok(new { id = await d.DispatchAsync<UpsertAdditionalGroupCommand, Guid>(new(establishmentId, id, r.Name, r.MinimumSelection, r.MaximumSelection, r.IsActive, r.Items.Select(x => new AdditionalGroupItemInput(x.AdditionalId, x.Order)).ToList()), ct) });
    private static CatalogResponse Map(CatalogReadModel m) => new(m.EstablishmentId, m.EstablishmentName, m.Slug, m.Categories.Select(c => new CategoryResponse(c.Id, c.ParentId, c.Name, c.Description, c.Order, c.ImageUrl, c.IsActive, c.Products.Select(p => new ProductResponse(p.Id, p.Code, p.Name, p.Description, p.BasePrice, p.IsFeatured, p.IsActive, p.AllowsNotes, p.Images.Select(i => new ProductImageResponse(i.Id, i.Url, i.Order, i.IsPrincipal)).ToList(), p.Variations.Select(v => new ProductVariationResponse(v.Id, v.Name, v.Price, v.Order, v.IsActive)).ToList(), p.AdditionalGroups.Select(g => new AdditionalGroupResponse(g.Id, g.Name, g.MinimumSelection, g.MaximumSelection, g.IsActive, g.Order, g.Items.Select(a => new AdditionalResponse(a.Id, a.Name, a.Price, a.IsActive, a.Order)).ToList())).ToList())).ToList())).ToList());
}
