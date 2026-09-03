namespace OrderHub.Contracts.PublicOrdering;

public sealed record PublicContextResponse(
    string EstablishmentName,
    string Slug,
    PublicThemeResponse Theme,
    PublicTableResponse? Table,
    IReadOnlyList<PublicPaymentMethodResponse> PaymentMethods);

public sealed record PublicThemeResponse(string PrimaryColor, string SecondaryColor, string BackgroundColor, string TextColor, string FontFamily, string? LogoUrl);
public sealed record PublicTableResponse(string Code, string Token);
public sealed record PublicPaymentMethodResponse(Guid Id, string Code, string Name, bool IsOnline, bool AllowsChange);

public sealed record PublicCustomerRequest(string Name, string Phone, string? Email, PublicAddressRequest? Address);
public sealed record PublicAddressRequest(string Label, string Street, string Number, string? Complement, string Neighborhood, string City, string State, string PostalCode);
public sealed record PublicCustomerResponse(Guid CustomerId, Guid? AddressId);

public sealed record PublicOrderItemRequest(Guid ProductId, Guid? VariationId, decimal Quantity, string? Notes, IReadOnlyList<PublicOrderAdditionalRequest> Additionals);
public sealed record PublicOrderAdditionalRequest(Guid AdditionalId, decimal Quantity);
public sealed record PublicOrderSimulationRequest(string ServiceType, Guid? CustomerId, Guid? CustomerAddressId, string? TableToken, PublicAddressRequest? DeliveryAddress, string? CouponCode, Guid? PaymentMethodId, IReadOnlyList<PublicOrderItemRequest> Items);
public sealed record PublicOrderSimulationResponse(decimal Subtotal, decimal Discount, decimal Fees, decimal Total, string? CouponCode, IReadOnlyList<PublicOrderItemResponse> Items);
public sealed record PublicOrderItemResponse(string ProductName, string? VariationName, decimal UnitPrice, decimal Quantity, decimal Total, IReadOnlyList<PublicOrderAdditionalResponse> Additionals);
public sealed record PublicOrderAdditionalResponse(string Name, decimal UnitPrice, decimal Quantity);

public sealed record PublicOrderConfirmationRequest(string ServiceType, Guid? CustomerId, Guid? CustomerAddressId, string? TableToken, PublicAddressRequest? DeliveryAddress, string? CouponCode, Guid PaymentMethodId, decimal? ReceivedAmount, IReadOnlyList<PublicOrderItemRequest> Items);
public sealed record PublicOrderConfirmationResponse(string Reference, long Number, string Status, decimal Total);
public sealed record PublicOrderTrackingResponse(string Reference, long Number, string ServiceType, string Status, decimal Subtotal, decimal Discount, decimal Fees, decimal Total, string? CouponCode, IReadOnlyList<PublicOrderItemResponse> Items, IReadOnlyList<PublicOrderHistoryResponse> History);
public sealed record PublicOrderHistoryResponse(string Status, DateTimeOffset OccurredAt, string? Note);
public sealed record PublicOrderCancellationRequest(string? Reason);
