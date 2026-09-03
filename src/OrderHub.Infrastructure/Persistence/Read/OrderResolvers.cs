using Dapper;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Abstractions.Persistence;
using OrderHub.Domain.Ordering;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Infrastructure.Persistence.Read;

public sealed class OrderOfferResolver(IReadConnectionFactory connectionFactory) : IOrderOfferResolver
{
    public async Task<OrderOfferSnapshot?> ResolveAsync(Guid tenantId, Guid establishmentId, Guid productId, Guid? variationId, IReadOnlyCollection<OrderAdditionalSelection> additionals, CancellationToken cancellationToken)
    {
        const string offerSql = """
            select p.id as ProductId, p.name as ProductName, v.id as VariationId, v.name as VariationName,
                   coalesce(v.price, p.base_price) as UnitPrice
            from catalog.product p
            left join catalog.product_variation v on v.product_id = p.id and v.id = @VariationId and v.is_active
            where p.tenant_id = @TenantId and p.establishment_id = @EstablishmentId and p.id = @ProductId and p.is_active
              and ((@VariationId is null and not exists (select 1 from catalog.product_variation pv where pv.product_id = p.id and pv.is_active)) or v.id is not null);
            """;
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<OfferRow>(new CommandDefinition(offerSql, new { TenantId = tenantId, EstablishmentId = establishmentId, ProductId = productId, VariationId = variationId }, cancellationToken: cancellationToken));
        if (row is null) return null;

        var ids = additionals.Select(x => x.AdditionalId).Distinct().ToArray();
        if (ids.Length != additionals.Count) return null;
        const string groupsSql = """
            select g.id as GroupId,g.minimum_selection as MinimumSelection,g.maximum_selection as MaximumSelection,gi.additional_id as AdditionalId
            from catalog.product_additional_group pg
            join catalog.additional_group g on g.tenant_id=pg.tenant_id and g.establishment_id=pg.establishment_id and g.id=pg.group_id and g.is_active
            left join catalog.additional_group_item gi on gi.tenant_id=g.tenant_id and gi.establishment_id=g.establishment_id and gi.group_id=g.id
            where pg.tenant_id=@TenantId and pg.establishment_id=@EstablishmentId and pg.product_id=@ProductId;
            """;
        var groupRows=(await connection.QueryAsync<GroupRow>(new CommandDefinition(groupsSql,new { TenantId=tenantId,EstablishmentId=establishmentId,ProductId=productId },cancellationToken:cancellationToken))).ToArray();
        foreach(var group in groupRows.GroupBy(x=>new{x.GroupId,x.MinimumSelection,x.MaximumSelection}))
        {
            var allowed=group.Where(x=>x.AdditionalId is not null).Select(x=>x.AdditionalId!.Value).ToHashSet();
            var count=additionals.Where(x=>allowed.Contains(x.AdditionalId)).Sum(x=>x.Quantity);
            if(count<group.Key.MinimumSelection || count>group.Key.MaximumSelection) return null;
        }
        IReadOnlyList<OrderAdditionalSnapshot> snapshots = [];
        if (ids.Length > 0)
        {
            const string additionalSql = """
                select distinct a.id as AdditionalId, a.name, a.price as UnitPrice
                from catalog.additional a
                join catalog.additional_group_item gi on gi.tenant_id = a.tenant_id and gi.establishment_id = a.establishment_id and gi.additional_id = a.id
                join catalog.additional_group g on g.tenant_id = gi.tenant_id and g.establishment_id = gi.establishment_id and g.id = gi.group_id and g.is_active
                join catalog.product_additional_group pg on pg.tenant_id = g.tenant_id and pg.establishment_id = g.establishment_id and pg.group_id = g.id and pg.product_id = @ProductId
                where a.tenant_id = @TenantId and a.establishment_id = @EstablishmentId and a.id = any(@Ids) and a.is_active;
                """;
            var additionalRows = (await connection.QueryAsync<AdditionalRow>(new CommandDefinition(additionalSql, new { TenantId = tenantId, EstablishmentId = establishmentId, ProductId = productId, Ids = ids }, cancellationToken: cancellationToken))).ToDictionary(x => x.AdditionalId);
            if (additionalRows.Count != ids.Length) return null;
            snapshots = additionals.Select(selection => new OrderAdditionalSnapshot(selection.AdditionalId, additionalRows[selection.AdditionalId].Name, new Money(additionalRows[selection.AdditionalId].UnitPrice), new Quantity(selection.Quantity))).ToArray();
        }
        return new(row.ProductId, row.VariationId, row.ProductName, row.VariationName, new Money(row.UnitPrice), snapshots);
    }
    private sealed record OfferRow(Guid ProductId, string ProductName, Guid? VariationId, string? VariationName, decimal UnitPrice);
    private sealed record AdditionalRow(Guid AdditionalId, string Name, decimal UnitPrice);
    private sealed record GroupRow(Guid GroupId, int MinimumSelection, int MaximumSelection, Guid? AdditionalId);
}

public sealed class OrderCustomerResolver(IReadConnectionFactory connectionFactory) : IOrderCustomerResolver
{
    public async Task<OrderCustomerSnapshot?> ResolveAsync(Guid tenantId, Guid establishmentId, Guid customerId, Guid? addressId, CancellationToken cancellationToken)
    {
        const string sql = """
            select c.id as CustomerId, c.name, c.phone, a.id as AddressId, a.street, a.number, a.complement,
                   a.neighborhood, a.city, a.state, a.postal_code as PostalCode
            from customers.customer c
            left join customers.customer_address a on a.tenant_id = c.tenant_id and a.establishment_id = c.establishment_id
                and a.customer_id = c.id and a.id = @AddressId
            where c.tenant_id = @TenantId and c.establishment_id = @EstablishmentId and c.id = @CustomerId
              and (@AddressId is null or a.id is not null);
            """;
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<CustomerRow>(new CommandDefinition(sql, new { TenantId = tenantId, EstablishmentId = establishmentId, CustomerId = customerId, AddressId = addressId }, cancellationToken: cancellationToken));
        if (row is null) return null;
        var address = row.AddressId is null ? null : new DeliveryAddressSnapshot(row.Street!, row.Number!, row.Complement, row.Neighborhood!, row.City!, row.State!, row.PostalCode!);
        return new(row.CustomerId, row.Name, row.Phone, row.AddressId, address);
    }
    private sealed record CustomerRow(Guid CustomerId, string Name, string Phone, Guid? AddressId, string? Street, string? Number, string? Complement, string? Neighborhood, string? City, string? State, string? PostalCode);
}

public sealed class OrderTableResolver(IReadConnectionFactory connectionFactory) : IOrderTableResolver
{
    public async Task<OrderTableSnapshot?> ResolveActiveAsync(Guid tenantId, Guid establishmentId, Guid tableId, CancellationToken cancellationToken)
    {
        const string sql = "select id as TableId, code from operations.service_table where tenant_id = @TenantId and establishment_id = @EstablishmentId and id = @TableId and is_active;";
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<OrderTableSnapshot>(new CommandDefinition(sql, new { TenantId = tenantId, EstablishmentId = establishmentId, TableId = tableId }, cancellationToken: cancellationToken));
    }
}
