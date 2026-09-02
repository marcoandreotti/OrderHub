namespace OrderHub.Contracts.Catalog;

public sealed record UpsertCategoryRequest(string Name, string? Description, int Order, string? ImageUrl, Guid? ParentId, bool IsActive);
public sealed record ProductImageRequest(string Url, int Order, bool IsPrincipal);
public sealed record ProductVariationRequest(string Name, decimal Price, int Order, bool IsActive);
public sealed record ProductGroupRequest(Guid GroupId, int Order);
public sealed record UpsertProductRequest(Guid CategoryId, string Code, string Name, string? Description, decimal BasePrice, bool IsFeatured, bool IsActive, bool AllowsNotes, IReadOnlyList<ProductImageRequest> Images, IReadOnlyList<ProductVariationRequest> Variations, IReadOnlyList<ProductGroupRequest> AdditionalGroups);
public sealed record UpsertAdditionalRequest(string Name, decimal Price, bool IsActive);
public sealed record AdditionalGroupItemRequest(Guid AdditionalId, int Order);
public sealed record UpsertAdditionalGroupRequest(string Name, int MinimumSelection, int MaximumSelection, bool IsActive, IReadOnlyList<AdditionalGroupItemRequest> Items);
public sealed record CatalogResponse(Guid EstablishmentId, string EstablishmentName, string Slug, IReadOnlyList<CategoryResponse> Categories);
public sealed record CategoryResponse(Guid Id, Guid? ParentId, string Name, string? Description, int Order, string? ImageUrl, bool IsActive, IReadOnlyList<ProductResponse> Products);
public sealed record ProductResponse(Guid Id, string Code, string Name, string? Description, decimal BasePrice, bool IsFeatured, bool IsActive, bool AllowsNotes, IReadOnlyList<ProductImageResponse> Images, IReadOnlyList<ProductVariationResponse> Variations, IReadOnlyList<AdditionalGroupResponse> AdditionalGroups);
public sealed record ProductImageResponse(Guid Id, string Url, int Order, bool IsPrincipal);
public sealed record ProductVariationResponse(Guid Id, string Name, decimal Price, int Order, bool IsActive);
public sealed record AdditionalGroupResponse(Guid Id, string Name, int MinimumSelection, int MaximumSelection, bool IsActive, int Order, IReadOnlyList<AdditionalResponse> Items);
public sealed record AdditionalResponse(Guid Id, string Name, decimal Price, bool IsActive, int Order);
