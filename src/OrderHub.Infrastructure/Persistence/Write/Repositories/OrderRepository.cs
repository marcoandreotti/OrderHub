using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderHub.Application.Abstractions.Ordering;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ordering;

namespace OrderHub.Infrastructure.Persistence.Write.Repositories;

public sealed class OrderRepository(OrderHubDbContext context) : IOrderRepository
{
    public Task<Order?> GetAsync(Guid tenantId, Guid establishmentId, Guid id, CancellationToken cancellationToken) =>
        context.Orders.Include(x => x.Items).ThenInclude(x => x.Additionals).Include(x => x.History)
            .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Id == id, cancellationToken);

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    { await context.Orders.AddAsync(order, cancellationToken); await SaveChangesAsync(cancellationToken); }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try { await context.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException) { throw new ConflictException("Order was changed by another operation."); }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }) { throw new ConflictException("Order number or public reference already exists."); }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.CheckViolation }) { throw new ConflictException("Order data violates establishment integrity."); }
    }
}
