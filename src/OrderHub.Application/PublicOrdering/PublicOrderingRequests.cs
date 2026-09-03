using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.PublicOrdering;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Domain.Ordering;

namespace OrderHub.Application.PublicOrdering;

public sealed record GetPublicContextQuery(string Slug, string? TableToken) : IQuery<PublicOrderingContext>;
public sealed record UpsertPublicCustomerCommand(string Slug, string Name, string Phone, string? Email, PublicAddress? Address) : ICommand<PublicCustomerResult>;
public sealed record PublicCustomerResult(Guid CustomerId, Guid? AddressId);
public sealed record SimulatePublicOrderQuery(string Slug, OrderServiceType ServiceType, Guid? CustomerId, Guid? CustomerAddressId, string? TableToken, PublicAddress? DeliveryAddress, string? CouponCode, Guid? PaymentMethodId, IReadOnlyCollection<PublicOrderLine> Items) : IQuery<PublicSimulation>;
public sealed record ConfirmPublicOrderCommand(string Slug, string IdempotencyKey, OrderServiceType ServiceType, Guid? CustomerId, Guid? CustomerAddressId, string? TableToken, PublicAddress? DeliveryAddress, string? CouponCode, Guid PaymentMethodId, decimal? ReceivedAmount, IReadOnlyCollection<PublicOrderLine> Items) : ICommand<PublicConfirmation>;
public sealed record GetPublicOrderQuery(string Reference) : IQuery<OrderReadModel>;
public sealed record CancelPublicOrderCommand(string Reference, string? Reason) : ICommand;
