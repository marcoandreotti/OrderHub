using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderHub.Application.Abstractions.PublicOrdering;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ordering;

namespace OrderHub.Infrastructure.Persistence.Write;

/// <summary>
/// Representa um repositório de solicitações de pedidos públicos que fornece métodos para encontrar e adicionar solicitações de pedidos públicos no banco de dados.
/// </summary>
public sealed class PublicOrderRequestRepository(OrderHubDbContext context) : IPublicOrderRequestRepository
{
    public Task<PublicOrderRequest?> FindAsync(Guid tenantId, Guid establishmentId, string key, CancellationToken cancellationToken) => context.PublicOrderRequests.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Key == key, cancellationToken);

    public async Task AddAsync(PublicOrderRequest request, CancellationToken cancellationToken)
    { try { await context.PublicOrderRequests.AddAsync(request, cancellationToken); await context.SaveChangesAsync(cancellationToken); } catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }) { throw new ConflictException("Idempotency key is already in use."); } }
}

public sealed class PublicOrderTransaction(OrderHubDbContext context) : IPublicOrderTransaction
{
    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    { if (context.Database.CurrentTransaction is not null) return await operation(cancellationToken); await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken); var result = await operation(cancellationToken); await transaction.CommitAsync(cancellationToken); return result; }
}