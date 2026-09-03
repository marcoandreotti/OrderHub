using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Customers;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.Payments;
using OrderHub.Application.Abstractions.Promotions;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.Customers;
using OrderHub.Application.Identity;
using OrderHub.Application.Ordering;
using OrderHub.Application.Payments;
using OrderHub.Application.Promotions;
using OrderHub.Contracts.Administration;
using OrderHub.Domain.Ordering;
using OrderHub.Domain.Promotions;

namespace OrderHub.Api.Administration;

internal static class AdministrationEndpoints
{
    /// <summary>Registra endpoints autenticados de operação e gestão por estabelecimento.</summary>
    public static IEndpointRouteBuilder MapAdministrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var root=endpoints.MapGroup("/api/admin/establishments/{establishmentId:guid}").WithTags("Administration");
        MapCustomers(root); MapOrders(root); MapCoupons(root); MapPayments(root);
        return endpoints;
    }

    private static void MapCustomers(RouteGroupBuilder root)
    {
        var group=root.MapGroup("/customers").RequireAuthorization(AdministrativePolicies.CustomerOperations);
        group.MapGet("",SearchCustomersAsync).Produces<PagedResponse<CustomerResponse>>();
        group.MapPost("",(Guid establishmentId,CustomerUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>UpsertCustomerAsync(establishmentId,null,r,d,ct));
        group.MapPut("/{customerId:guid}",(Guid establishmentId,Guid customerId,CustomerUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>UpsertCustomerAsync(establishmentId,customerId,r,d,ct));
        group.MapPost("/{customerId:guid}/addresses",(Guid establishmentId,Guid customerId,CustomerAddressUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>UpsertAddressAsync(establishmentId,customerId,null,r,d,ct));
        group.MapPut("/{customerId:guid}/addresses/{addressId:guid}",(Guid establishmentId,Guid customerId,Guid addressId,CustomerAddressUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>UpsertAddressAsync(establishmentId,customerId,addressId,r,d,ct));
        group.MapDelete("/{customerId:guid}/addresses/{addressId:guid}",RemoveAddressAsync);
    }

    private static void MapOrders(RouteGroupBuilder root)
    {
        var read=root.MapGroup("/orders").RequireAuthorization(AdministrativePolicies.OrderRead);
        read.MapGet("",SearchOrdersAsync).Produces<PagedResponse<OrderSummaryResponse>>(); read.MapGet("/{orderId:guid}",GetOrderAsync).Produces<OrderDetailResponse>();
        root.MapPost("/orders/{orderId:guid}/prepare",(Guid establishmentId,Guid orderId,OrderTransitionRequest r,ICommandDispatcher d,CancellationToken ct)=>TransitionAsync(establishmentId,orderId,OrderStatus.Preparing,r,d,ct)).RequireAuthorization(AdministrativePolicies.OrderKitchen);
        root.MapPost("/orders/{orderId:guid}/ready",(Guid establishmentId,Guid orderId,OrderTransitionRequest r,ICommandDispatcher d,CancellationToken ct)=>TransitionAsync(establishmentId,orderId,OrderStatus.Ready,r,d,ct)).RequireAuthorization(AdministrativePolicies.OrderKitchen);
        root.MapPost("/orders/{orderId:guid}/dispatch",(Guid establishmentId,Guid orderId,OrderTransitionRequest r,ICommandDispatcher d,CancellationToken ct)=>TransitionAsync(establishmentId,orderId,OrderStatus.OutForDelivery,r,d,ct)).RequireAuthorization(AdministrativePolicies.OrderDelivery);
        root.MapPost("/orders/{orderId:guid}/complete",(Guid establishmentId,Guid orderId,OrderTransitionRequest r,ICommandDispatcher d,CancellationToken ct)=>TransitionAsync(establishmentId,orderId,OrderStatus.Completed,r,d,ct)).RequireAuthorization(AdministrativePolicies.OrderCompletion);
        root.MapPost("/orders/{orderId:guid}/cancel",(Guid establishmentId,Guid orderId,OrderTransitionRequest r,ICommandDispatcher d,CancellationToken ct)=>TransitionAsync(establishmentId,orderId,OrderStatus.Cancelled,r,d,ct)).RequireAuthorization(AdministrativePolicies.OrderAttendance);
        root.MapPost("/orders/{orderId:guid}/reject",(Guid establishmentId,Guid orderId,OrderTransitionRequest r,ICommandDispatcher d,CancellationToken ct)=>TransitionAsync(establishmentId,orderId,OrderStatus.Rejected,r,d,ct)).RequireAuthorization(AdministrativePolicies.OrderAttendance);
    }

    private static void MapCoupons(RouteGroupBuilder root)
    {
        var group=root.MapGroup("/coupons").RequireAuthorization(AdministrativePolicies.PromotionManagement);
        group.MapGet("",ListCouponsAsync).Produces<PagedResponse<CouponResponse>>();
        group.MapPost("",(Guid establishmentId,CouponUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>UpsertCouponAsync(establishmentId,null,r,d,ct));
        group.MapPut("/{couponId:guid}",(Guid establishmentId,Guid couponId,CouponUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>UpsertCouponAsync(establishmentId,couponId,r,d,ct));
        group.MapPatch("/{couponId:guid}/active",SetCouponActiveAsync);
    }

    private static void MapPayments(RouteGroupBuilder root)
    {
        var methods=root.MapGroup("/payment-methods").RequireAuthorization(AdministrativePolicies.PaymentManagement);
        methods.MapGet("",ListPaymentMethodsAsync).Produces<PagedResponse<PaymentMethodResponse>>();
        methods.MapPost("",(Guid establishmentId,PaymentMethodUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>UpsertPaymentMethodAsync(establishmentId,null,r,d,ct));
        methods.MapPut("/{methodId:guid}",(Guid establishmentId,Guid methodId,PaymentMethodUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>UpsertPaymentMethodAsync(establishmentId,methodId,r,d,ct));
        methods.MapPatch("/{methodId:guid}/active",SetPaymentMethodActiveAsync);
        var payments=root.MapGroup("/orders/{orderId:guid}/payments").RequireAuthorization(AdministrativePolicies.PaymentOperations);
        payments.MapGet("",GetPaymentsAsync).Produces<OrderPaymentsResponse>(); payments.MapPost("",CreatePaymentAsync);
        payments.MapPost("/{paymentId:guid}/confirm",ConfirmPaymentAsync); payments.MapPost("/{paymentId:guid}/fail",FailPaymentAsync); payments.MapPost("/{paymentId:guid}/cancel",CancelPaymentAsync);
    }

    private static async Task<IResult> SearchCustomersAsync(Guid establishmentId,string? search,int? page,int? pageSize,IQueryDispatcher d,CancellationToken ct){var currentPage=page??1;var currentSize=pageSize??20;var x=await d.DispatchAsync<SearchCustomersQuery,CustomerSearchResult>(new(establishmentId,search,currentPage,currentSize),ct);return Results.Ok(new PagedResponse<CustomerResponse>(currentPage,currentSize,x.Total,x.Items.Select(Map).ToArray()));}
    private static async Task<IResult> UpsertCustomerAsync(Guid establishmentId,Guid? id,CustomerUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>Results.Ok(new{id=await d.DispatchAsync<UpsertCustomerCommand,Guid>(new(establishmentId,id,r.Name,r.Phone,r.Email),ct)});
    private static async Task<IResult> UpsertAddressAsync(Guid establishmentId,Guid customerId,Guid? addressId,CustomerAddressUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>Results.Ok(new{id=await d.DispatchAsync<UpsertCustomerAddressCommand,Guid>(new(establishmentId,customerId,addressId,r.Label,r.Street,r.Number,r.Complement,r.Neighborhood,r.City,r.State,r.PostalCode,r.IsPrimary),ct)});
    private static async Task<IResult> RemoveAddressAsync(Guid establishmentId,Guid customerId,Guid addressId,ICommandDispatcher d,CancellationToken ct){await d.DispatchAsync(new RemoveCustomerAddressCommand(establishmentId,customerId,addressId),ct);return Results.NoContent();}
    private static async Task<IResult> SearchOrdersAsync(Guid establishmentId,DateTimeOffset? from,DateTimeOffset? to,OrderStatus? status,long? number,OrderServiceType? serviceType,int? page,int? pageSize,IQueryDispatcher d,CancellationToken ct){var currentPage=page??1;var currentSize=pageSize??20;var x=await d.DispatchAsync<SearchOrdersQuery,OrderSearchResult>(new(establishmentId,from,to,status,number,serviceType,currentPage,currentSize),ct);return Results.Ok(new PagedResponse<OrderSummaryResponse>(currentPage,currentSize,x.Total,x.Items.Select(o=>new OrderSummaryResponse(o.Id,o.Number,o.ServiceType.ToString(),o.Status.ToString(),o.CustomerName,o.CustomerPhone,o.Total,o.CreatedAt)).ToArray()));}
    private static async Task<IResult> GetOrderAsync(Guid establishmentId,Guid orderId,IQueryDispatcher d,CancellationToken ct)=>Results.Ok(Map(await d.DispatchAsync<GetOrderQuery,OrderReadModel>(new(establishmentId,orderId),ct)));
    private static async Task<IResult> TransitionAsync(Guid establishmentId,Guid orderId,OrderStatus status,OrderTransitionRequest r,ICommandDispatcher d,CancellationToken ct){await d.DispatchAsync(new TransitionOrderCommand(establishmentId,orderId,status,r.Note),ct);return Results.NoContent();}
    private static async Task<IResult> ListCouponsAsync(Guid establishmentId,string? search,bool? isActive,int? page,int? pageSize,IQueryDispatcher d,CancellationToken ct){var currentPage=page??1;var currentSize=pageSize??20;var x=await d.DispatchAsync<SearchCouponsQuery,CouponSearchResult>(new(establishmentId,search,isActive,currentPage,currentSize),ct);return Results.Ok(new PagedResponse<CouponResponse>(currentPage,currentSize,x.Total,x.Items.Select(c=>new CouponResponse(c.Id,c.Code,c.Description,c.DiscountType.ToString(),c.Value,c.MinimumOrder,c.StartsAt,c.EndsAt,c.MaximumUses,c.UsedCount,c.IsActive)).ToArray()));}
    private static async Task<IResult> UpsertCouponAsync(Guid establishmentId,Guid? id,CouponUpsertRequest r,ICommandDispatcher d,CancellationToken ct){var type=Enum.TryParse<CouponDiscountType>(r.DiscountType,true,out var parsed)?parsed:(CouponDiscountType)(-1);return Results.Ok(new{id=await d.DispatchAsync<UpsertCouponCommand,Guid>(new(establishmentId,id,r.Code,r.Description,type,r.Value,r.MinimumOrder,r.StartsAt,r.EndsAt,r.MaximumUses),ct)});}
    private static async Task<IResult> SetCouponActiveAsync(Guid establishmentId,Guid couponId,SetActiveRequest r,ICommandDispatcher d,CancellationToken ct){await d.DispatchAsync(new SetCouponActiveCommand(establishmentId,couponId,r.IsActive),ct);return Results.NoContent();}
    private static async Task<IResult> ListPaymentMethodsAsync(Guid establishmentId,string? search,bool? isActive,int? page,int? pageSize,IQueryDispatcher d,CancellationToken ct){var currentPage=page??1;var currentSize=pageSize??20;var x=await d.DispatchAsync<SearchPaymentMethodsQuery,PaymentMethodSearchResult>(new(establishmentId,search,isActive,currentPage,currentSize),ct);return Results.Ok(new PagedResponse<PaymentMethodResponse>(currentPage,currentSize,x.Total,x.Items.Select(m=>new PaymentMethodResponse(m.Id,m.Code,m.Name,m.IsOnline,m.AllowsChange,m.IsActive)).ToArray()));}
    private static async Task<IResult> UpsertPaymentMethodAsync(Guid establishmentId,Guid? id,PaymentMethodUpsertRequest r,ICommandDispatcher d,CancellationToken ct)=>Results.Ok(new{id=await d.DispatchAsync<UpsertPaymentMethodCommand,Guid>(new(establishmentId,id,r.Code,r.Name,r.IsOnline,r.AllowsChange),ct)});
    private static async Task<IResult> SetPaymentMethodActiveAsync(Guid establishmentId,Guid methodId,SetActiveRequest r,ICommandDispatcher d,CancellationToken ct){await d.DispatchAsync(new SetPaymentMethodActiveCommand(establishmentId,methodId,r.IsActive),ct);return Results.NoContent();}
    private static async Task<IResult> GetPaymentsAsync(Guid establishmentId,Guid orderId,IQueryDispatcher d,CancellationToken ct)=>Results.Ok(Map(await d.DispatchAsync<GetOrderPaymentsQuery,OrderPaymentsReadModel>(new(establishmentId,orderId),ct)));
    private static async Task<IResult> CreatePaymentAsync(Guid establishmentId,Guid orderId,PaymentCreateRequest r,ICommandDispatcher d,CancellationToken ct)=>Results.Ok(new{id=await d.DispatchAsync<CreatePaymentCommand,Guid>(new(establishmentId,orderId,r.PaymentMethodId,r.Amount,r.ReceivedAmount),ct)});
    private static async Task<IResult> ConfirmPaymentAsync(Guid establishmentId,Guid paymentId,PaymentConfirmRequest r,ICommandDispatcher d,CancellationToken ct)=>Results.Ok(new{id=await d.DispatchAsync<ConfirmPaymentCommand,Guid>(new(establishmentId,paymentId,r.Amount,r.IdempotencyKey,r.ExternalId),ct)});
    private static async Task<IResult> FailPaymentAsync(Guid establishmentId,Guid paymentId,ICommandDispatcher d,CancellationToken ct){await d.DispatchAsync(new FailPaymentCommand(establishmentId,paymentId),ct);return Results.NoContent();}
    private static async Task<IResult> CancelPaymentAsync(Guid establishmentId,Guid paymentId,ICommandDispatcher d,CancellationToken ct){await d.DispatchAsync(new CancelPaymentCommand(establishmentId,paymentId),ct);return Results.NoContent();}
    private static CustomerResponse Map(CustomerReadModel c)=>new(c.Id,c.Name,c.Phone,c.Email,c.Addresses.Select(a=>new CustomerAddressResponse(a.Id,a.Label,a.Street,a.Number,a.Complement,a.Neighborhood,a.City,a.State,a.PostalCode,a.IsPrimary)).ToArray());
    private static OrderDetailResponse Map(OrderReadModel o)=>new(o.Id,o.Number,o.PublicReference,o.ServiceType.ToString(),o.Status.ToString(),o.CustomerName,o.CustomerPhone,o.TableCode,o.Subtotal,o.Discount,o.Fees,o.Total,o.CouponCode,o.Items.Select(i=>new AdminOrderItemResponse(i.Id,i.ProductName,i.VariationName,i.UnitPrice,i.Quantity,i.Total,i.Notes,i.Additionals.Select(a=>new AdminOrderAdditionalResponse(a.Name,a.UnitPrice,a.Quantity)).ToArray())).ToArray(),o.History.Select(h=>new AdminOrderHistoryResponse(h.PreviousStatus.ToString(),h.NewStatus.ToString(),h.OccurredAt,h.ActorId,h.Note)).ToArray());
    private static OrderPaymentsResponse Map(OrderPaymentsReadModel o)=>new(o.OrderId,o.DueAmount,o.ConfirmedAmount,o.IsFullyCovered,o.OperationalStatus,o.Payments.Select(p=>new PaymentResponse(p.Id,p.MethodCode,p.MethodName,p.Amount,p.ReceivedAmount,p.Change,p.Status.ToString(),p.ExternalId,p.CreatedAt,p.ConfirmedAt)).ToArray());
}
