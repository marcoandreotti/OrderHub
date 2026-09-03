using OrderHub.Application.Abstractions.Catalog;
using OrderHub.Application.Abstractions.Customers;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.Payments;
using OrderHub.Application.Abstractions.Promotions;
using OrderHub.Application.Abstractions.PublicOrdering;
using OrderHub.Application.Exceptions;
using OrderHub.Application.PublicOrdering;
using OrderHub.Domain.Ordering;
using OrderHub.Domain.Payments;
using OrderHub.Domain.Promotions;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Application.Tests.PublicOrdering;

public sealed class PublicOrderingApplicationTests
{
    [Fact]
    public async Task Inactive_or_unknown_slug_is_not_revealed()
    {
        var handler=new GetPublicContextQueryHandler(new ContextGateway(null));
        await Assert.ThrowsAsync<NotFoundException>(()=>handler.HandleAsync(new("closed",null),CancellationToken.None));
    }

    [Fact]
    public async Task Simulation_uses_authoritative_offer_price()
    {
        var context=Scope();
        var handler=new SimulatePublicOrderQueryHandler(new ContextGateway(context),new OfferResolver(25m),new CustomerResolver(),new TableResolver(),new CouponRepository(),new PaymentMethodRepository(),TimeProvider.System);
        var result=await handler.HandleAsync(new("unit-a",OrderServiceType.Pickup,null,null,null,null,null,null,[new(Guid.NewGuid(),null,2,null,[])]),CancellationToken.None);
        Assert.Equal(50m,result.Total); Assert.Equal(25m,result.Items.Single().UnitPrice);
    }

    [Fact]
    public void Confirmation_requires_strong_idempotency_key()
    {
        var validator=new ConfirmPublicOrderCommandValidator();
        var command=new ConfirmPublicOrderCommand("unit-a","short",OrderServiceType.Pickup,null,null,null,null,null,Guid.NewGuid(),null,[new(Guid.NewGuid(),null,1,null,[])]);
        var result=validator.Validate(command);
        Assert.Contains(result.Errors,x=>x.PropertyName=="IdempotencyKey");
    }

    [Fact]
    public void Tracking_reference_rejects_enumerable_identifiers()
    {
        var query=new GetPublicOrderQuery(Guid.NewGuid().ToString());
        var result=new GetPublicOrderQueryValidator().Validate(query);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Repeated_confirmation_returns_original_order_without_duplication()
    {
        var scope=Scope(); var method=PaymentMethod.Create(scope.TenantId,scope.EstablishmentId,"PIX","Pix",true,false,DateTimeOffset.UtcNow);
        var orders=new OrderRepository(); var requests=new RequestRepository();
        var handler=new ConfirmPublicOrderCommandHandler(new ContextGateway(scope),new OfferResolver(20m),new CustomerResolver(),new TableResolver(),new CouponRepository(),new ActivePaymentMethodRepository(method),new PaymentRepository(),orders,new Sequence(),requests,new Transaction(),TimeProvider.System);
        var command=new ConfirmPublicOrderCommand("unit-a","confirmation-key-123",OrderServiceType.Pickup,null,null,null,null,null,method.Id,null,[new(Guid.NewGuid(),null,1,null,[])]);
        var first=await handler.HandleAsync(command,CancellationToken.None); var replay=await handler.HandleAsync(command,CancellationToken.None);
        Assert.Equal(first,replay); Assert.Equal(1,orders.AddCount);
    }

    private static PublicOrderingContext Scope()=>new(Guid.NewGuid(),Guid.NewGuid(),"Unit","unit-a","#1","#2","#3","#4","Arial",null,null,null,null,[]);
    private sealed class ContextGateway(PublicOrderingContext? value):IPublicOrderingContextGateway { public Task<PublicOrderingContext?> ResolveAsync(string normalizedSlug,string? tableToken,CancellationToken cancellationToken)=>Task.FromResult(value); }
    private sealed class OfferResolver(decimal price):IOrderOfferResolver { public Task<OrderOfferSnapshot?> ResolveAsync(Guid tenantId,Guid establishmentId,Guid productId,Guid? variationId,IReadOnlyCollection<OrderAdditionalSelection> additionals,CancellationToken cancellationToken)=>Task.FromResult<OrderOfferSnapshot?>(new(productId,variationId,"Pizza",null,new Money(price),[])); }
    private sealed class CustomerResolver:IOrderCustomerResolver { public Task<OrderCustomerSnapshot?> ResolveAsync(Guid tenantId,Guid establishmentId,Guid customerId,Guid? addressId,CancellationToken cancellationToken)=>Task.FromResult<OrderCustomerSnapshot?>(null); }
    private sealed class TableResolver:IOrderTableResolver { public Task<OrderTableSnapshot?> ResolveActiveAsync(Guid tenantId,Guid establishmentId,Guid tableId,CancellationToken cancellationToken)=>Task.FromResult<OrderTableSnapshot?>(null); }
    private sealed class CouponRepository:ICouponRepository { public Task<Coupon?> GetAsync(Guid a,Guid b,Guid c,CancellationToken d)=>Task.FromResult<Coupon?>(null); public Task<Coupon?> FindByCodeAsync(Guid a,Guid b,string c,CancellationToken d)=>Task.FromResult<Coupon?>(null); public Task AddAsync(Coupon a,CancellationToken b)=>Task.CompletedTask; public Task SaveChangesAsync(CancellationToken a)=>Task.CompletedTask; }
    private sealed class PaymentMethodRepository:IPaymentMethodRepository { public Task<PaymentMethod?> GetAsync(Guid a,Guid b,Guid c,CancellationToken d)=>Task.FromResult<PaymentMethod?>(null); public Task<bool> CodeExistsAsync(Guid a,Guid b,string c,Guid? d,CancellationToken e)=>Task.FromResult(false); public Task AddAsync(PaymentMethod a,CancellationToken b)=>Task.CompletedTask; public Task SaveChangesAsync(CancellationToken a)=>Task.CompletedTask; }
    private sealed class ActivePaymentMethodRepository(PaymentMethod method):IPaymentMethodRepository { public Task<PaymentMethod?> GetAsync(Guid a,Guid b,Guid c,CancellationToken d)=>Task.FromResult<PaymentMethod?>(c==method.Id?method:null); public Task<bool> CodeExistsAsync(Guid a,Guid b,string c,Guid? d,CancellationToken e)=>Task.FromResult(false); public Task AddAsync(PaymentMethod a,CancellationToken b)=>Task.CompletedTask; public Task SaveChangesAsync(CancellationToken a)=>Task.CompletedTask; }
    private sealed class PaymentRepository:IPaymentRepository { public Task<Payment?> GetAsync(Guid a,Guid b,Guid c,CancellationToken d)=>Task.FromResult<Payment?>(null); public Task AddAsync(Payment a,CancellationToken b)=>Task.CompletedTask; public Task SaveChangesAsync(CancellationToken a)=>Task.CompletedTask; }
    private sealed class OrderRepository:IOrderRepository { private readonly Dictionary<Guid,Order> values=[]; public int AddCount{get;private set;} public Task<Order?> GetAsync(Guid a,Guid b,Guid id,CancellationToken d)=>Task.FromResult(values.GetValueOrDefault(id)); public Task AddAsync(Order order,CancellationToken b){values.Add(order.Id,order);AddCount++;return Task.CompletedTask;} public Task SaveChangesAsync(CancellationToken a)=>Task.CompletedTask; }
    private sealed class Sequence:IOrderNumberSequence { private long number; public Task<long> ReserveAsync(Guid a,Guid b,CancellationToken c)=>Task.FromResult(++number); }
    private sealed class RequestRepository:IPublicOrderRequestRepository { private PublicOrderRequest? value; public Task<PublicOrderRequest?> FindAsync(Guid a,Guid b,string c,CancellationToken d)=>Task.FromResult(value?.Key==c?value:null); public Task AddAsync(PublicOrderRequest request,CancellationToken b){value=request;return Task.CompletedTask;} }
    private sealed class Transaction:IPublicOrderTransaction { public Task<T> ExecuteAsync<T>(Func<CancellationToken,Task<T>> operation,CancellationToken token)=>operation(token); }
}
