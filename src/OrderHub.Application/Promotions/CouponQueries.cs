using OrderHub.Application.Abstractions.Promotions;
using OrderHub.Application.Abstractions.Queries;

namespace OrderHub.Application.Promotions;

public sealed record ListCouponsQuery(Guid EstablishmentId) : IQuery<IReadOnlyList<CouponReadModel>>;
public sealed record SearchCouponsQuery(Guid EstablishmentId,string? Search,bool? IsActive,int Page=1,int PageSize=20):IQuery<CouponSearchResult>;
