namespace OrderHub.Contracts.Administration;

public sealed record PagedResponse<T>(int Page, int PageSize, int Total, IReadOnlyList<T> Items);

public sealed record CustomerUpsertRequest(string Name, string Phone, string? Email);
public sealed record CustomerAddressUpsertRequest(string Label, string Street, string Number, string? Complement, string Neighborhood, string City, string State, string PostalCode, bool IsPrimary);
public sealed record CustomerResponse(Guid Id, string Name, string Phone, string? Email, IReadOnlyList<CustomerAddressResponse> Addresses);
public sealed record CustomerAddressResponse(Guid Id, string Label, string Street, string Number, string? Complement, string Neighborhood, string City, string State, string PostalCode, bool IsPrimary);

public sealed record OrderSummaryResponse(Guid Id, long Number, string ServiceType, string Status, string? CustomerName, string? CustomerPhone, decimal Total, DateTimeOffset CreatedAt);
public sealed record OrderTransitionRequest(string? Note);
public sealed record OrderDetailResponse(Guid Id, long? Number, string? PublicReference, string ServiceType, string Status, string? CustomerName, string? CustomerPhone, string? TableCode, decimal Subtotal, decimal Discount, decimal Fees, decimal Total, string? CouponCode, IReadOnlyList<AdminOrderItemResponse> Items, IReadOnlyList<AdminOrderHistoryResponse> History);
public sealed record AdminOrderItemResponse(Guid Id, string ProductName, string? VariationName, decimal UnitPrice, decimal Quantity, decimal Total, string? Notes, IReadOnlyList<AdminOrderAdditionalResponse> Additionals);
public sealed record AdminOrderAdditionalResponse(string Name, decimal UnitPrice, decimal Quantity);
public sealed record AdminOrderHistoryResponse(string PreviousStatus, string NewStatus, DateTimeOffset OccurredAt, Guid? ActorId, string? Note);

public sealed record CouponUpsertRequest(string Code, string? Description, string DiscountType, decimal Value, decimal MinimumOrder, DateTimeOffset StartsAt, DateTimeOffset EndsAt, int? MaximumUses);
public sealed record CouponResponse(Guid Id, string Code, string? Description, string DiscountType, decimal Value, decimal MinimumOrder, DateTimeOffset StartsAt, DateTimeOffset EndsAt, int? MaximumUses, int UsedCount, bool IsActive);
public sealed record SetActiveRequest(bool IsActive);

public sealed record PaymentMethodUpsertRequest(string Code, string Name, bool IsOnline, bool AllowsChange);
public sealed record PaymentMethodResponse(Guid Id, string Code, string Name, bool IsOnline, bool AllowsChange, bool IsActive);
public sealed record PaymentCreateRequest(Guid PaymentMethodId, decimal Amount, decimal? ReceivedAmount);
public sealed record PaymentConfirmRequest(decimal Amount, string IdempotencyKey, string? ExternalId);
public sealed record PaymentResponse(Guid Id, string MethodCode, string MethodName, decimal Amount, decimal? ReceivedAmount, decimal Change, string Status, string? ExternalId, DateTimeOffset CreatedAt, DateTimeOffset? ConfirmedAt);
public sealed record OrderPaymentsResponse(Guid OrderId, decimal DueAmount, decimal ConfirmedAmount, bool IsFullyCovered, string OperationalStatus, IReadOnlyList<PaymentResponse> Payments);
