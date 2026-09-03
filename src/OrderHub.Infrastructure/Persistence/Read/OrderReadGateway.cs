using Dapper;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.Persistence;
using OrderHub.Domain.Ordering;

namespace OrderHub.Infrastructure.Persistence.Read;

/// <summary>
/// Representa um gateway de leitura para pedidos (orders) que fornece métodos para pesquisar e obter informações detalhadas sobre pedidos.
/// </summary>
public sealed class OrderReadGateway(IReadConnectionFactory connectionFactory) : IOrderReadGateway
{
    public async Task<OrderSearchResult> SearchAsync(Guid tenantId, Guid establishmentId, DateTimeOffset? from, DateTimeOffset? to, OrderStatus? status, long? number, OrderServiceType? serviceType, int page, int pageSize, CancellationToken cancellationToken)
    {
        const string sql = """
            select count(*) from orders."order" o
            where o.tenant_id=@TenantId and o.establishment_id=@EstablishmentId
              and (@From is null or o.created_at>=@From) and (@To is null or o.created_at<@To)
              and (@Status is null or o.status=@Status) and (@Number is null or o.number=@Number)
              and (@ServiceType is null or o.service_type=@ServiceType);
            select o.id,o.number,o.service_type as ServiceType,o.status,o.customer_name as CustomerName,o.customer_phone as CustomerPhone,o.total,o.created_at as CreatedAt
            from orders."order" o
            where o.tenant_id=@TenantId and o.establishment_id=@EstablishmentId
              and (@From is null or o.created_at>=@From) and (@To is null or o.created_at<@To)
              and (@Status is null or o.status=@Status) and (@Number is null or o.number=@Number)
              and (@ServiceType is null or o.service_type=@ServiceType)
            order by o.created_at desc,o.id desc offset @Offset rows fetch next @PageSize rows only;
            """;
        var parameters = new { TenantId = tenantId, EstablishmentId = establishmentId, From = from, To = to, Status = status?.ToString(), Number = number, ServiceType = serviceType?.ToString(), Offset = (page - 1) * pageSize, PageSize = pageSize };
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken); using var grid = await connection.QueryMultipleAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        var total = await grid.ReadSingleAsync<int>(); var rows = (await grid.ReadAsync<SummaryRow>()).ToArray();
        return new(total, rows.Select(x => new OrderSummaryReadModel(x.Id, x.Number, Enum.Parse<OrderServiceType>(x.ServiceType), Enum.Parse<OrderStatus>(x.Status), x.CustomerName, x.CustomerPhone, x.Total, new DateTimeOffset(DateTime.SpecifyKind(x.CreatedAt, DateTimeKind.Utc)))).ToArray());
    }

    /// <summary>
    /// Obtém informações detalhadas de um pedido específico com base no ID do inquilino, ID do estabelecimento e ID do pedido.
    /// </summary>
    /// <returns></returns>
    public async Task<OrderReadModel?> GetAsync(Guid tenantId, Guid establishmentId, Guid orderId, CancellationToken cancellationToken)
    {
        const string sql = """
            select o.id, o.number, o.public_reference as PublicReference, o.service_type as ServiceType, o.status,
                   o.customer_name as CustomerName, o.customer_phone as CustomerPhone, t.code as TableCode,
                   o.delivery_street as DeliveryStreet, o.delivery_number as DeliveryNumber, o.delivery_complement as DeliveryComplement,
                   o.delivery_neighborhood as DeliveryNeighborhood, o.delivery_city as DeliveryCity, o.delivery_state as DeliveryState,
                   o.delivery_postal_code as DeliveryPostalCode, o.subtotal, o.discount, o.fees, o.total,
                   o.coupon_code as CouponCode, case when o.coupon_code is null then 0 else o.discount end as CouponDiscount
            from orders."order" o
            left join operations.service_table t on t.tenant_id = o.tenant_id and t.establishment_id = o.establishment_id and t.id = o.table_id
            where o.tenant_id = @TenantId and o.establishment_id = @EstablishmentId and o.id = @OrderId;

            select i.id, i.product_name as ProductName, i.variation_name as VariationName, i.unit_price as UnitPrice,
                   i.quantity, i.total, i.notes
            from orders.order_item i
            where i.tenant_id = @TenantId and i.establishment_id = @EstablishmentId and i.order_id = @OrderId
            order by i.id;

            select a.order_item_id as OrderItemId, a.name, a.unit_price as UnitPrice, a.quantity
            from orders.order_item_additional a
            join orders.order_item i on i.tenant_id = a.tenant_id and i.establishment_id = a.establishment_id and i.id = a.order_item_id
            where a.tenant_id = @TenantId and a.establishment_id = @EstablishmentId and i.order_id = @OrderId
            order by a.id;

            select h.previous_status as PreviousStatus, h.new_status as NewStatus, h.occurred_at as OccurredAt, h.actor_id as ActorId, h.note
            from orders.order_status_history h
            where h.tenant_id = @TenantId and h.establishment_id = @EstablishmentId and h.order_id = @OrderId
            order by h.occurred_at, h.id;
            """;
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(sql, new { TenantId = tenantId, EstablishmentId = establishmentId, OrderId = orderId }, cancellationToken: cancellationToken));
        var order = await grid.ReadSingleOrDefaultAsync<OrderRow>(); if (order is null) return null;
        var items = (await grid.ReadAsync<ItemRow>()).ToArray(); var additionals = (await grid.ReadAsync<AdditionalRow>()).ToLookup(x => x.OrderItemId); var history = (await grid.ReadAsync<HistoryRow>()).ToArray();
        var address = order.DeliveryStreet is null ? null : new DeliveryAddressSnapshot(order.DeliveryStreet, order.DeliveryNumber!, order.DeliveryComplement, order.DeliveryNeighborhood!, order.DeliveryCity!, order.DeliveryState!, order.DeliveryPostalCode!);
        return new(order.Id, order.Number, order.PublicReference, Enum.Parse<OrderServiceType>(order.ServiceType), Enum.Parse<OrderStatus>(order.Status), order.CustomerName, order.CustomerPhone, order.TableCode, address,
            order.Subtotal, order.Discount, order.Fees, order.Total, order.CouponCode, order.CouponDiscount,
            items.Select(item => new OrderItemReadModel(item.Id, item.ProductName, item.VariationName, item.UnitPrice, item.Quantity, item.Total, item.Notes, additionals[item.Id].Select(a => new OrderAdditionalReadModel(a.Name, a.UnitPrice, a.Quantity)).ToArray())).ToArray(),
            history.Select(item => new OrderHistoryReadModel(Enum.Parse<OrderStatus>(item.PreviousStatus), Enum.Parse<OrderStatus>(item.NewStatus), item.OccurredAt, item.ActorId, item.Note)).ToArray());
    }

    private sealed record OrderRow(Guid Id, long? Number, string? PublicReference, string ServiceType, string Status, string? CustomerName, string? CustomerPhone, string? TableCode, string? DeliveryStreet, string? DeliveryNumber, string? DeliveryComplement, string? DeliveryNeighborhood, string? DeliveryCity, string? DeliveryState, string? DeliveryPostalCode, decimal Subtotal, decimal Discount, decimal Fees, decimal Total, string? CouponCode, decimal CouponDiscount);
    private sealed record ItemRow(Guid Id, string ProductName, string? VariationName, decimal UnitPrice, decimal Quantity, decimal Total, string? Notes);
    private sealed record AdditionalRow(Guid OrderItemId, string Name, decimal UnitPrice, decimal Quantity);

    private sealed class SummaryRow
    { public Guid Id { get; set; } public long Number { get; set; } public string ServiceType { get; set; } = string.Empty; public string Status { get; set; } = string.Empty; public string? CustomerName { get; set; } public string? CustomerPhone { get; set; } public decimal Total { get; set; } public DateTime CreatedAt { get; set; } }

    private sealed class HistoryRow
    {
        public string PreviousStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTimeOffset OccurredAt { get; set; }
        public Guid? ActorId { get; set; }
        public string? Note { get; set; }
    }
}