using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.Exceptions;
using OrderHub.Application.Tenancy;
using OrderHub.Domain.Ordering;
using OrderHub.Domain.SharedKernel;
using FluentValidation.Results;
using OrderHub.Application.Abstractions.Promotions;
using OrderHub.Domain.Promotions;

namespace OrderHub.Application.Ordering;

public sealed class CreateOrderDraftCommandHandler(
    EstablishmentScopeResolver scopeResolver,
    IOrderRepository repository,
    IOrderCustomerResolver customerResolver,
    IOrderTableResolver tableResolver,
    TimeProvider timeProvider) : ICommandHandler<CreateOrderDraftCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateOrderDraftCommand command, CancellationToken cancellationToken)
    {
        var scope = await scopeResolver.ResolveAsync(command.EstablishmentId, cancellationToken);
        OrderCustomerSnapshot? customer = null;
        if (command.CustomerId is { } customerId)
        {
            customer = await customerResolver.ResolveAsync(scope.TenantId, scope.EstablishmentId, customerId, command.CustomerAddressId, cancellationToken)
                ?? throw new NotFoundException("Customer was not found in this establishment.");
        }

        if (command.ServiceType == OrderServiceType.Table)
        {
            if (command.TableId is not { } tableId || await tableResolver.ResolveActiveAsync(scope.TenantId, scope.EstablishmentId, tableId, cancellationToken) is null)
                throw new NotFoundException("Active table was not found in this establishment.");
        }

        var address = command.DeliveryAddress is null
            ? customer?.DeliveryAddress
            : new DeliveryAddressSnapshot(command.DeliveryAddress.Street, command.DeliveryAddress.Number, command.DeliveryAddress.Complement, command.DeliveryAddress.Neighborhood, command.DeliveryAddress.City, command.DeliveryAddress.State, command.DeliveryAddress.PostalCode);
        var order = Order.Create(scope.TenantId, scope.EstablishmentId, command.ServiceType, customer?.CustomerId, customer?.Name, customer?.Phone, command.TableId, address, timeProvider.GetUtcNow());
        await repository.AddAsync(order, cancellationToken);
        return order.Id;
    }
}

public sealed class AddOrderItemCommandHandler(
    EstablishmentScopeResolver scopeResolver,
    IOrderRepository repository,
    IOrderOfferResolver offerResolver,
    TimeProvider timeProvider) : ICommandHandler<AddOrderItemCommand, Guid>
{
    public async Task<Guid> HandleAsync(AddOrderItemCommand command, CancellationToken cancellationToken)
    {
        var scope = await scopeResolver.ResolveAsync(command.EstablishmentId, cancellationToken);
        var order = await repository.GetAsync(scope.TenantId, scope.EstablishmentId, command.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order was not found.");
        var offer = await offerResolver.ResolveAsync(scope.TenantId, scope.EstablishmentId, command.ProductId, command.VariationId, command.Additionals, cancellationToken)
            ?? throw new NotFoundException("Sellable offer was not found in this establishment.");
        var item = order.AddItem(offer.ProductId, offer.VariationId, offer.ProductName, offer.VariationName, offer.UnitPrice, new Quantity(command.Quantity),
            offer.Additionals.Select(x => new OrderAdditionalInput(x.AdditionalId, x.Name, x.UnitPrice, x.Quantity)).ToArray(), command.Notes, timeProvider.GetUtcNow());
        await repository.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}

public sealed class ConfirmOrderCommandHandler(
    EstablishmentScopeResolver scopeResolver,
    IOrderRepository repository,
    IOrderOfferResolver offerResolver,
    IOrderNumberSequence sequence,
    IOrderConfirmationTransaction transaction,
    ICouponRepository coupons,
    TimeProvider timeProvider) : ICommandHandler<ConfirmOrderCommand>
{
    public async Task HandleAsync(ConfirmOrderCommand command, CancellationToken cancellationToken)
    {
        var scope = await scopeResolver.ResolveAsync(command.EstablishmentId, cancellationToken);
        var order = await repository.GetAsync(scope.TenantId, scope.EstablishmentId, command.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order was not found.");

        foreach (var item in order.Items)
        {
            var selections = item.Additionals.Select(x => new OrderAdditionalSelection(x.AdditionalId, x.Quantity.Value)).ToArray();
            var current = await offerResolver.ResolveAsync(scope.TenantId, scope.EstablishmentId, item.ProductId, item.VariationId, selections, cancellationToken);
            if (current is null || !Matches(item, current))
                throw new ConflictException("Order offer changed or is no longer available. Recompose the order before confirming.");
        }

        Coupon? coupon = null;
        if (order.CouponId is { } couponId)
        {
            coupon = await coupons.GetAsync(scope.TenantId, scope.EstablishmentId, couponId, cancellationToken) ?? throw new ConflictException("Applied coupon no longer exists.");
            var evaluation = coupon.Evaluate(order.Subtotal, timeProvider.GetUtcNow());
            if (evaluation.Code != order.CouponCode || evaluation.Discount != order.Discount) throw new ConflictException("Applied coupon changed. Apply it again before confirming.");
        }

        await transaction.ExecuteAsync(async token =>
        {
            coupon?.Consume(order.Id, order.Subtotal, timeProvider.GetUtcNow());
            var number = await sequence.ReserveAsync(scope.TenantId, scope.EstablishmentId, token);
            order.Confirm(number, timeProvider.GetUtcNow(), scope.UserId);
            await repository.SaveChangesAsync(token);
        }, cancellationToken);
    }

    private static bool Matches(OrderItem item, OrderOfferSnapshot current) =>
        item.ProductName == current.ProductName && item.VariationName == current.VariationName && item.UnitPrice == current.UnitPrice
        && item.Additionals.Count == current.Additionals.Count
        && item.Additionals.All(saved => current.Additionals.Any(value => value.AdditionalId == saved.AdditionalId && value.Name == saved.Name && value.UnitPrice == saved.UnitPrice && value.Quantity == saved.Quantity));
}

public sealed class TransitionOrderCommandHandler(
    EstablishmentScopeResolver scopeResolver,
    IOrderRepository repository,
    TimeProvider timeProvider) : ICommandHandler<TransitionOrderCommand>
{
    public async Task HandleAsync(TransitionOrderCommand command, CancellationToken cancellationToken)
    {
        var scope = await scopeResolver.ResolveAsync(command.EstablishmentId, cancellationToken);
        var order = await repository.GetAsync(scope.TenantId, scope.EstablishmentId, command.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order was not found.");
        var now = timeProvider.GetUtcNow();
        switch (command.NewStatus)
        {
            case OrderStatus.Preparing: order.StartPreparation(now, scope.UserId); break;
            case OrderStatus.Ready: order.MarkReady(now, scope.UserId); break;
            case OrderStatus.OutForDelivery: order.Dispatch(now, scope.UserId); break;
            case OrderStatus.Completed: order.Complete(now, scope.UserId); break;
            case OrderStatus.Cancelled: order.Cancel(now, scope.UserId, command.Note); break;
            case OrderStatus.Rejected: order.Reject(now, scope.UserId, command.Note); break;
            default: throw new ValidationException([new ValidationFailure(nameof(command.NewStatus), "Requested order status is not an operational transition.")]);
        }
        await repository.SaveChangesAsync(cancellationToken);
    }
}

public sealed class GetOrderQueryHandler(EstablishmentScopeResolver scopeResolver, IOrderReadGateway gateway) : IQueryHandler<GetOrderQuery, OrderReadModel>
{
    public async Task<OrderReadModel> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeResolver.ResolveAsync(query.EstablishmentId, cancellationToken);
        return await gateway.GetAsync(scope.TenantId, scope.EstablishmentId, query.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order was not found.");
    }
}

public sealed class SearchOrdersQueryHandler(EstablishmentScopeResolver scopeResolver, IOrderReadGateway gateway) : IQueryHandler<SearchOrdersQuery, OrderSearchResult>
{
    public async Task<OrderSearchResult> HandleAsync(SearchOrdersQuery query, CancellationToken cancellationToken)
    { var scope=await scopeResolver.ResolveAsync(query.EstablishmentId,cancellationToken); return await gateway.SearchAsync(scope.TenantId,scope.EstablishmentId,query.From,query.To,query.Status,query.Number,query.ServiceType,query.Page,query.PageSize,cancellationToken); }
}
