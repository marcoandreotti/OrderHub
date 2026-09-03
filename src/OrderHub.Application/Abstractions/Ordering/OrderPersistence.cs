using OrderHub.Domain.Ordering;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Application.Abstractions.Ordering;

public interface IOrderRepository
{
    Task<Order?> GetAsync(Guid tenantId, Guid establishmentId, Guid id, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IOrderNumberSequence
{
    Task<long> ReserveAsync(Guid tenantId, Guid establishmentId, CancellationToken cancellationToken);
}

public interface IOrderConfirmationTransaction
{
    Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken);
}

public interface IOrderOfferResolver
{
    Task<OrderOfferSnapshot?> ResolveAsync(
        Guid tenantId,
        Guid establishmentId,
        Guid productId,
        Guid? variationId,
        IReadOnlyCollection<OrderAdditionalSelection> additionals,
        CancellationToken cancellationToken);
}

public interface IOrderCustomerResolver
{
    Task<OrderCustomerSnapshot?> ResolveAsync(Guid tenantId, Guid establishmentId, Guid customerId, Guid? addressId, CancellationToken cancellationToken);
}

public interface IOrderTableResolver
{
    Task<OrderTableSnapshot?> ResolveActiveAsync(Guid tenantId, Guid establishmentId, Guid tableId, CancellationToken cancellationToken);
}

public interface IOrderReadGateway
{
    Task<OrderReadModel?> GetAsync(Guid tenantId, Guid establishmentId, Guid orderId, CancellationToken cancellationToken);
    Task<OrderSearchResult> SearchAsync(Guid tenantId, Guid establishmentId, DateTimeOffset? from, DateTimeOffset? to, OrderStatus? status, long? number, OrderServiceType? serviceType, int page, int pageSize, CancellationToken cancellationToken);
}

public sealed record OrderAdditionalSelection(Guid AdditionalId, decimal Quantity);
public sealed record OrderAdditionalSnapshot(Guid AdditionalId, string Name, Money UnitPrice, Quantity Quantity);
public sealed record OrderOfferSnapshot(Guid ProductId, Guid? VariationId, string ProductName, string? VariationName, Money UnitPrice, IReadOnlyList<OrderAdditionalSnapshot> Additionals);
public sealed record OrderTableSnapshot(Guid TableId, string Code);
public sealed record OrderCustomerSnapshot(Guid CustomerId, string Name, string Phone, Guid? AddressId, DeliveryAddressSnapshot? DeliveryAddress);

public sealed record OrderReadModel(
    Guid Id,
    long? Number,
    string? PublicReference,
    OrderServiceType ServiceType,
    OrderStatus Status,
    string? CustomerName,
    string? CustomerPhone,
    string? TableCode,
    DeliveryAddressSnapshot? DeliveryAddress,
    decimal Subtotal,
    decimal Discount,
    decimal Fees,
    decimal Total,
    string? CouponCode,
    decimal CouponDiscount,
    IReadOnlyList<OrderItemReadModel> Items,
    IReadOnlyList<OrderHistoryReadModel> History);

public sealed record OrderItemReadModel(Guid Id, string ProductName, string? VariationName, decimal UnitPrice, decimal Quantity, decimal Total, string? Notes, IReadOnlyList<OrderAdditionalReadModel> Additionals);
public sealed record OrderAdditionalReadModel(string Name, decimal UnitPrice, decimal Quantity);
public sealed record OrderHistoryReadModel(OrderStatus PreviousStatus, OrderStatus NewStatus, DateTimeOffset OccurredAt, Guid? ActorId, string? Note);
public sealed record OrderSearchResult(int Total, IReadOnlyList<OrderSummaryReadModel> Items);
public sealed record OrderSummaryReadModel(Guid Id, long Number, OrderServiceType ServiceType, OrderStatus Status, string? CustomerName, string? CustomerPhone, decimal Total, DateTimeOffset CreatedAt);
