using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Domain.Ordering;

namespace OrderHub.Application.Ordering;

public sealed record GetOrderQuery(Guid EstablishmentId, Guid OrderId) : IQuery<OrderReadModel>;
public sealed record SearchOrdersQuery(Guid EstablishmentId, DateTimeOffset? From, DateTimeOffset? To, OrderStatus? Status, long? Number, OrderServiceType? ServiceType, int Page = 1, int PageSize = 20) : IQuery<OrderSearchResult>;
