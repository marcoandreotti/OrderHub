using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Domain.Ordering;

namespace OrderHub.Application.Abstractions.PublicOrdering;

public interface IPublicOrderingContextGateway
{
    Task<PublicOrderingContext?> ResolveAsync(string normalizedSlug, string? tableToken, CancellationToken cancellationToken);
}

public interface IPublicOrderRequestRepository
{
    Task<PublicOrderRequest?> FindAsync(Guid tenantId, Guid establishmentId, string key, CancellationToken cancellationToken);
    Task AddAsync(PublicOrderRequest request, CancellationToken cancellationToken);
}

public interface IPublicOrderLocator
{
    Task<PublicOrderLocation?> FindAsync(string reference, CancellationToken cancellationToken);
}

public interface IPublicOrderTransaction
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken);
}

public sealed record PublicOrderingContext(Guid TenantId, Guid EstablishmentId, string EstablishmentName, string Slug, string PrimaryColor, string SecondaryColor, string BackgroundColor, string TextColor, string FontFamily, string? LogoUrl, Guid? TableId, string? TableCode, string? TableToken, IReadOnlyList<PublicPaymentMethod> PaymentMethods);
public sealed record PublicPaymentMethod(Guid Id, string Code, string Name, bool IsOnline, bool AllowsChange);
public sealed record PublicOrderLocation(Guid TenantId, Guid EstablishmentId, Guid OrderId);
public sealed record PublicOrderLine(Guid ProductId, Guid? VariationId, decimal Quantity, string? Notes, IReadOnlyCollection<OrderAdditionalSelection> Additionals);
public sealed record PublicAddress(string Label, string Street, string Number, string? Complement, string Neighborhood, string City, string State, string PostalCode);
public sealed record PublicSimulation(decimal Subtotal, decimal Discount, decimal Fees, decimal Total, string? CouponCode, IReadOnlyList<PublicSimulationItem> Items);
public sealed record PublicSimulationItem(string ProductName, string? VariationName, decimal UnitPrice, decimal Quantity, decimal Total, IReadOnlyList<PublicSimulationAdditional> Additionals);
public sealed record PublicSimulationAdditional(string Name, decimal UnitPrice, decimal Quantity);
public sealed record PublicConfirmation(string Reference, long Number, OrderStatus Status, decimal Total);
