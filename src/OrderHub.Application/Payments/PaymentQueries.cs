using OrderHub.Application.Abstractions.Payments;
using OrderHub.Application.Abstractions.Queries;

namespace OrderHub.Application.Payments;
public sealed record ListPaymentMethodsQuery(Guid EstablishmentId) : IQuery<IReadOnlyList<PaymentMethodReadModel>>;
public sealed record GetOrderPaymentsQuery(Guid EstablishmentId, Guid OrderId) : IQuery<OrderPaymentsReadModel>;
public sealed record SearchPaymentMethodsQuery(Guid EstablishmentId,string? Search,bool? IsActive,int Page=1,int PageSize=20):IQuery<PaymentMethodSearchResult>;
