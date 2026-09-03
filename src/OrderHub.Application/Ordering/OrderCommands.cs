using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Domain.Ordering;

namespace OrderHub.Application.Ordering;

public sealed record DeliveryAddressInput(string Street, string Number, string? Complement, string Neighborhood, string City, string State, string PostalCode);

public sealed record CreateOrderDraftCommand(
    Guid EstablishmentId,
    OrderServiceType ServiceType,
    Guid? CustomerId,
    Guid? CustomerAddressId,
    Guid? TableId,
    DeliveryAddressInput? DeliveryAddress) : ICommand<Guid>;

public sealed record AddOrderItemCommand(
    Guid EstablishmentId,
    Guid OrderId,
    Guid ProductId,
    Guid? VariationId,
    decimal Quantity,
    IReadOnlyCollection<OrderAdditionalSelection> Additionals,
    string? Notes) : ICommand<Guid>;

public sealed record ConfirmOrderCommand(Guid EstablishmentId, Guid OrderId) : ICommand;

public sealed record TransitionOrderCommand(
    Guid EstablishmentId,
    Guid OrderId,
    OrderStatus NewStatus,
    string? Note = null) : ICommand;
