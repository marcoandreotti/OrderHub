using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.PublicOrdering;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.PublicOrdering;
using OrderHub.Contracts.PublicOrdering;
using OrderHub.Domain.Ordering;

namespace OrderHub.Api.PublicOrdering;

internal static class PublicOrderingEndpoints
{
    /// <summary>Registra a borda HTTP anônima de composição e acompanhamento de pedidos.</summary>
    public static IEndpointRouteBuilder MapPublicOrderingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group=endpoints.MapGroup("/api/public/ordering").AllowAnonymous().WithTags("Public Ordering");
        group.MapGet("/{slug}/context",GetContextAsync).Produces<PublicContextResponse>().ProducesProblem(404);
        group.MapPost("/{slug}/customers",UpsertCustomerAsync).Produces<PublicCustomerResponse>().ProducesValidationProblem();
        group.MapPost("/{slug}/simulate",SimulateAsync).Produces<PublicOrderSimulationResponse>().ProducesValidationProblem();
        group.MapPost("/{slug}/orders",ConfirmAsync).Produces<PublicOrderConfirmationResponse>(StatusCodes.Status201Created).ProducesValidationProblem().ProducesProblem(409);
        group.MapGet("/orders/{reference}",TrackAsync).Produces<PublicOrderTrackingResponse>().ProducesProblem(404);
        group.MapPost("/orders/{reference}/cancel",CancelAsync).Produces(StatusCodes.Status204NoContent).ProducesProblem(422);
        return endpoints;
    }

    private static async Task<IResult> GetContextAsync(string slug,string? tableToken,IQueryDispatcher dispatcher,CancellationToken ct)
    { var x=await dispatcher.DispatchAsync<GetPublicContextQuery,PublicOrderingContext>(new(slug,tableToken),ct); return Results.Ok(new PublicContextResponse(x.EstablishmentName,x.Slug,new(x.PrimaryColor,x.SecondaryColor,x.BackgroundColor,x.TextColor,x.FontFamily,x.LogoUrl),x.TableId is null?null:new(x.TableCode!,x.TableToken!),x.PaymentMethods.Select(m=>new PublicPaymentMethodResponse(m.Id,m.Code,m.Name,m.IsOnline,m.AllowsChange)).ToArray())); }

    private static async Task<IResult> UpsertCustomerAsync(string slug,PublicCustomerRequest request,ICommandDispatcher dispatcher,CancellationToken ct)
    { var x=await dispatcher.DispatchAsync<UpsertPublicCustomerCommand,PublicCustomerResult>(new(slug,request.Name,request.Phone,request.Email,Map(request.Address)),ct); return Results.Ok(new PublicCustomerResponse(x.CustomerId,x.AddressId)); }

    private static async Task<IResult> SimulateAsync(string slug,PublicOrderSimulationRequest request,IQueryDispatcher dispatcher,CancellationToken ct)
    { var x=await dispatcher.DispatchAsync<SimulatePublicOrderQuery,PublicSimulation>(new(slug,ParseServiceType(request.ServiceType),request.CustomerId,request.CustomerAddressId,request.TableToken,Map(request.DeliveryAddress),request.CouponCode,request.PaymentMethodId,request.Items.Select(Map).ToArray()),ct); return Results.Ok(Map(x)); }

    private static async Task<IResult> ConfirmAsync(string slug,PublicOrderConfirmationRequest request,HttpRequest http,ICommandDispatcher dispatcher,CancellationToken ct)
    { var key=http.Headers["Idempotency-Key"].ToString(); var x=await dispatcher.DispatchAsync<ConfirmPublicOrderCommand,PublicConfirmation>(new(slug,key,ParseServiceType(request.ServiceType),request.CustomerId,request.CustomerAddressId,request.TableToken,Map(request.DeliveryAddress),request.CouponCode,request.PaymentMethodId,request.ReceivedAmount,request.Items.Select(Map).ToArray()),ct); var response=new PublicOrderConfirmationResponse(x.Reference,x.Number,x.Status.ToString(),x.Total); return Results.Created($"/api/public/ordering/orders/{x.Reference}",response); }

    private static async Task<IResult> TrackAsync(string reference,IQueryDispatcher dispatcher,CancellationToken ct)
    { var x=await dispatcher.DispatchAsync<GetPublicOrderQuery,OrderReadModel>(new(reference),ct); return Results.Ok(MapTracking(x)); }

    private static async Task<IResult> CancelAsync(string reference,PublicOrderCancellationRequest request,ICommandDispatcher dispatcher,CancellationToken ct)
    { await dispatcher.DispatchAsync(new CancelPublicOrderCommand(reference,request.Reason),ct); return Results.NoContent(); }

    private static OrderServiceType ParseServiceType(string value) => Enum.TryParse<OrderServiceType>(value,true,out var result)?result:(OrderServiceType)(-1);
    private static PublicAddress? Map(PublicAddressRequest? x)=>x is null?null:new(x.Label,x.Street,x.Number,x.Complement,x.Neighborhood,x.City,x.State,x.PostalCode);
    private static PublicOrderLine Map(PublicOrderItemRequest x)=>new(x.ProductId,x.VariationId,x.Quantity,x.Notes,x.Additionals.Select(a=>new OrderAdditionalSelection(a.AdditionalId,a.Quantity)).ToArray());
    private static PublicOrderSimulationResponse Map(PublicSimulation x)=>new(x.Subtotal,x.Discount,x.Fees,x.Total,x.CouponCode,x.Items.Select(i=>new PublicOrderItemResponse(i.ProductName,i.VariationName,i.UnitPrice,i.Quantity,i.Total,i.Additionals.Select(a=>new PublicOrderAdditionalResponse(a.Name,a.UnitPrice,a.Quantity)).ToArray())).ToArray());
    private static PublicOrderTrackingResponse MapTracking(OrderReadModel x)=>new(x.PublicReference!,x.Number!.Value,x.ServiceType.ToString(),x.Status.ToString(),x.Subtotal,x.Discount,x.Fees,x.Total,x.CouponCode,x.Items.Select(i=>new PublicOrderItemResponse(i.ProductName,i.VariationName,i.UnitPrice,i.Quantity,i.Total,i.Additionals.Select(a=>new PublicOrderAdditionalResponse(a.Name,a.UnitPrice,a.Quantity)).ToArray())).ToArray(),x.History.Select(h=>new PublicOrderHistoryResponse(h.NewStatus.ToString(),h.OccurredAt,h.Note)).ToArray());
}
