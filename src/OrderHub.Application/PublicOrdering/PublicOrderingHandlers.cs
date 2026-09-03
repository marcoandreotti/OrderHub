using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Customers;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.Payments;
using OrderHub.Application.Abstractions.Promotions;
using OrderHub.Application.Abstractions.PublicOrdering;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Customers;
using OrderHub.Domain.Ordering;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Application.PublicOrdering;

public sealed class GetPublicContextQueryHandler(IPublicOrderingContextGateway gateway) : IQueryHandler<GetPublicContextQuery, PublicOrderingContext>
{
    public async Task<PublicOrderingContext> HandleAsync(GetPublicContextQuery query, CancellationToken cancellationToken) =>
        await gateway.ResolveAsync(NormalizeSlug(query.Slug), NormalizeOptional(query.TableToken), cancellationToken)
        ?? throw new NotFoundException("Public ordering context was not found.");
    internal static string NormalizeSlug(string value) => value.Trim().ToLowerInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class UpsertPublicCustomerCommandHandler(IPublicOrderingContextGateway contexts, ICustomerRepository customers, TimeProvider timeProvider) : ICommandHandler<UpsertPublicCustomerCommand, PublicCustomerResult>
{
    public async Task<PublicCustomerResult> HandleAsync(UpsertPublicCustomerCommand command, CancellationToken cancellationToken)
    {
        var scope = await contexts.ResolveAsync(GetPublicContextQueryHandler.NormalizeSlug(command.Slug), null, cancellationToken) ?? throw new NotFoundException("Public ordering context was not found.");
        var normalizedPhone = Customer.NormalizePhone(command.Phone);
        var customer = await customers.FindByNormalizedPhoneAsync(scope.TenantId, scope.EstablishmentId, normalizedPhone, cancellationToken);
        var now = timeProvider.GetUtcNow();
        if (customer is null) { customer = Customer.Create(scope.TenantId, scope.EstablishmentId, command.Name, command.Phone, command.Email, now); await customers.AddAsync(customer, cancellationToken); }
        else { customer.UpdateContact(command.Name, command.Phone, command.Email, now); }
        Guid? addressId = null;
        if (command.Address is { } address)
        {
            var existing = customer.Addresses.FirstOrDefault(x => x.Label.Equals(address.Label.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existing is null) addressId = customer.AddAddress(address.Label, address.Street, address.Number, address.Complement, address.Neighborhood, address.City, address.State, address.PostalCode, true, now).Id;
            else { customer.UpdateAddress(existing.Id, address.Label, address.Street, address.Number, address.Complement, address.Neighborhood, address.City, address.State, address.PostalCode, true, now); addressId = existing.Id; }
        }
        await customers.SaveChangesAsync(cancellationToken);
        return new(customer.Id, addressId);
    }
}

public sealed class SimulatePublicOrderQueryHandler(IPublicOrderingContextGateway contexts, IOrderOfferResolver offers, IOrderCustomerResolver customers, IOrderTableResolver tables, ICouponRepository coupons, IPaymentMethodRepository paymentMethods, TimeProvider timeProvider) : IQueryHandler<SimulatePublicOrderQuery, PublicSimulation>
{
    public async Task<PublicSimulation> HandleAsync(SimulatePublicOrderQuery query, CancellationToken cancellationToken)
    {
        var scope = await PublicOrderComposer.ResolveScopeAsync(contexts, query.Slug, query.TableToken, cancellationToken);
        if (query.PaymentMethodId is { } methodId && (await paymentMethods.GetAsync(scope.TenantId, scope.EstablishmentId, methodId, cancellationToken) is not { IsActive: true })) throw new ConflictException("Payment method is not available.");
        var order = await PublicOrderComposer.ComposeAsync(scope, query.ServiceType, query.CustomerId, query.CustomerAddressId, query.DeliveryAddress, query.Items, offers, customers, tables, timeProvider.GetUtcNow(), cancellationToken);
        await PublicOrderComposer.ApplyCouponAsync(order, query.CouponCode, scope, coupons, timeProvider.GetUtcNow(), cancellationToken);
        return PublicOrderComposer.ToSimulation(order);
    }
}

public sealed class ConfirmPublicOrderCommandHandler(IPublicOrderingContextGateway contexts, IOrderOfferResolver offers, IOrderCustomerResolver customers, IOrderTableResolver tables, ICouponRepository coupons, IPaymentMethodRepository paymentMethods, IPaymentRepository payments, IOrderRepository orders, IOrderNumberSequence sequence, IPublicOrderRequestRepository requests, IPublicOrderTransaction transaction, TimeProvider timeProvider) : ICommandHandler<ConfirmPublicOrderCommand, PublicConfirmation>
{
    public async Task<PublicConfirmation> HandleAsync(ConfirmPublicOrderCommand command, CancellationToken cancellationToken)
    {
        var scope = await PublicOrderComposer.ResolveScopeAsync(contexts, command.Slug, command.TableToken, cancellationToken);
        var hash = Hash(command);
        var previous = await requests.FindAsync(scope.TenantId, scope.EstablishmentId, command.IdempotencyKey.Trim(), cancellationToken);
        if (previous is not null) return await ReplayAsync(previous, hash, scope, cancellationToken);
        return await transaction.ExecuteAsync(async token =>
        {
            previous = await requests.FindAsync(scope.TenantId, scope.EstablishmentId, command.IdempotencyKey.Trim(), token);
            if (previous is not null) return await ReplayAsync(previous, hash, scope, token);
            var method = await paymentMethods.GetAsync(scope.TenantId, scope.EstablishmentId, command.PaymentMethodId, token);
            if (method is not { IsActive: true }) throw new ConflictException("Payment method is not available.");
            var now = timeProvider.GetUtcNow();
            var order = await PublicOrderComposer.ComposeAsync(scope, command.ServiceType, command.CustomerId, command.CustomerAddressId, command.DeliveryAddress, command.Items, offers, customers, tables, now, token);
            var coupon = await PublicOrderComposer.ApplyCouponAsync(order, command.CouponCode, scope, coupons, now, token);
            var number = await sequence.ReserveAsync(scope.TenantId, scope.EstablishmentId, token); order.Confirm(number, now);
            await orders.AddAsync(order, token);
            coupon?.Consume(order.Id, order.Subtotal, now);
            if (coupon is not null) await coupons.SaveChangesAsync(token);
            await payments.AddAsync(Domain.Payments.Payment.Create(scope.TenantId, scope.EstablishmentId, order.Id, method, order.Total, command.ReceivedAmount is null ? null : new Money(command.ReceivedAmount.Value), now), token);
            await requests.AddAsync(PublicOrderRequest.Create(scope.TenantId, scope.EstablishmentId, command.IdempotencyKey, hash, order.Id, now), token);
            return new PublicConfirmation(order.PublicReference!, order.Number!.Value, order.Status, order.Total.Amount);
        }, cancellationToken);
    }

    private async Task<PublicConfirmation> ReplayAsync(PublicOrderRequest request, string hash, PublicOrderingContext scope, CancellationToken token)
    {
        if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(request.PayloadHash), Convert.FromHexString(hash))) throw new ConflictException("Idempotency key was already used with different content.");
        var order = await orders.GetAsync(scope.TenantId, scope.EstablishmentId, request.OrderId, token) ?? throw new ConflictException("Idempotent order result is unavailable.");
        return new(order.PublicReference!, order.Number!.Value, order.Status, order.Total.Amount);
    }

    private static string Hash(ConfirmPublicOrderCommand value)
    {
        var content = string.Join('|', value.Slug.Trim().ToLowerInvariant(), value.ServiceType, value.CustomerId, value.CustomerAddressId, value.TableToken?.Trim(), Address(value.DeliveryAddress), value.CouponCode?.Trim().ToUpperInvariant(), value.PaymentMethodId, value.ReceivedAmount?.ToString(CultureInfo.InvariantCulture), string.Join(';', value.Items.Select(x => $"{x.ProductId},{x.VariationId},{x.Quantity.ToString(CultureInfo.InvariantCulture)},{x.Notes},{string.Join(',', x.Additionals.OrderBy(a => a.AdditionalId).Select(a => $"{a.AdditionalId}:{a.Quantity.ToString(CultureInfo.InvariantCulture)}"))}")));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content))).ToLowerInvariant();
    }
    private static string Address(PublicAddress? x) => x is null ? string.Empty : $"{x.Label},{x.Street},{x.Number},{x.Complement},{x.Neighborhood},{x.City},{x.State},{x.PostalCode}";
}

public sealed class GetPublicOrderQueryHandler(IPublicOrderLocator locator, IOrderReadGateway orders) : IQueryHandler<GetPublicOrderQuery, OrderReadModel>
{
    public async Task<OrderReadModel> HandleAsync(GetPublicOrderQuery query, CancellationToken cancellationToken)
    { var location = await locator.FindAsync(query.Reference, cancellationToken) ?? throw new NotFoundException("Order was not found."); return await orders.GetAsync(location.TenantId, location.EstablishmentId, location.OrderId, cancellationToken) ?? throw new NotFoundException("Order was not found."); }
}

public sealed class CancelPublicOrderCommandHandler(IPublicOrderLocator locator, IOrderRepository orders, TimeProvider timeProvider) : ICommandHandler<CancelPublicOrderCommand>
{
    public async Task HandleAsync(CancelPublicOrderCommand command, CancellationToken cancellationToken)
    { var location = await locator.FindAsync(command.Reference, cancellationToken) ?? throw new NotFoundException("Order was not found."); var order = await orders.GetAsync(location.TenantId, location.EstablishmentId, location.OrderId, cancellationToken) ?? throw new NotFoundException("Order was not found."); if(order.Status!=OrderStatus.Confirmed) throw new Domain.Exceptions.DomainException("Order can no longer be cancelled by the customer."); order.Cancel(timeProvider.GetUtcNow(), null, command.Reason); await orders.SaveChangesAsync(cancellationToken); }
}

internal static class PublicOrderComposer
{
    public static async Task<PublicOrderingContext> ResolveScopeAsync(IPublicOrderingContextGateway contexts, string slug, string? tableToken, CancellationToken token) => await contexts.ResolveAsync(GetPublicContextQueryHandler.NormalizeSlug(slug), GetPublicContextQueryHandler.NormalizeOptional(tableToken), token) ?? throw new NotFoundException("Public ordering context was not found.");
    public static async Task<Order> ComposeAsync(PublicOrderingContext scope, OrderServiceType serviceType, Guid? customerId, Guid? addressId, PublicAddress? address, IReadOnlyCollection<PublicOrderLine> lines, IOrderOfferResolver offers, IOrderCustomerResolver customers, IOrderTableResolver tables, DateTimeOffset now, CancellationToken token)
    {
        OrderCustomerSnapshot? customer = null; if (customerId is { } id) customer = await customers.ResolveAsync(scope.TenantId, scope.EstablishmentId, id, addressId, token) ?? throw new NotFoundException("Customer was not found.");
        Guid? tableId = null; if (serviceType == OrderServiceType.Table) { tableId = scope.TableId; if (tableId is null || await tables.ResolveActiveAsync(scope.TenantId, scope.EstablishmentId, tableId.Value, token) is null) throw new NotFoundException("Active table was not found."); }
        var delivery = address is null ? customer?.DeliveryAddress : new DeliveryAddressSnapshot(address.Street, address.Number, address.Complement, address.Neighborhood, address.City, address.State, address.PostalCode);
        var order = Order.Create(scope.TenantId, scope.EstablishmentId, serviceType, customer?.CustomerId, customer?.Name, customer?.Phone, tableId, delivery, now);
        foreach (var line in lines) { var offer = await offers.ResolveAsync(scope.TenantId, scope.EstablishmentId, line.ProductId, line.VariationId, line.Additionals, token) ?? throw new ConflictException("One or more offers are unavailable."); order.AddItem(offer.ProductId, offer.VariationId, offer.ProductName, offer.VariationName, offer.UnitPrice, new Quantity(line.Quantity), offer.Additionals.Select(x => new OrderAdditionalInput(x.AdditionalId, x.Name, x.UnitPrice, x.Quantity)).ToArray(), line.Notes, now); }
        return order;
    }
    public static async Task<Domain.Promotions.Coupon?> ApplyCouponAsync(Order order, string? code, PublicOrderingContext scope, ICouponRepository coupons, DateTimeOffset now, CancellationToken token)
    { if (string.IsNullOrWhiteSpace(code)) return null; var coupon = await coupons.FindByCodeAsync(scope.TenantId, scope.EstablishmentId, Domain.Promotions.Coupon.NormalizeCode(code), token) ?? throw new NotFoundException("Coupon was not found."); var evaluation = coupon.Evaluate(order.Subtotal, now); order.ApplyCoupon(evaluation.CouponId, evaluation.Code, evaluation.Discount, now); return coupon; }
    public static PublicSimulation ToSimulation(Order order) => new(order.Subtotal.Amount, order.Discount.Amount, order.Fees.Amount, order.Total.Amount, order.CouponCode, order.Items.Select(x => new PublicSimulationItem(x.ProductName, x.VariationName, x.UnitPrice.Amount, x.Quantity.Value, x.Total.Amount, x.Additionals.Select(a => new PublicSimulationAdditional(a.Name, a.UnitPrice.Amount, a.Quantity.Value)).ToArray())).ToArray());
}
