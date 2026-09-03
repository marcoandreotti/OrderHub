using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderHub.Application.Abstractions.Payments;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Payments;

namespace OrderHub.Infrastructure.Persistence.Write.Repositories;

/// <summary>
/// Representa um repositório de métodos de pagamento que fornece métodos para obter, verificar a existência de códigos, adicionar e salvar alterações de métodos de pagamento no banco de dados.
/// </summary>
/// <param name="context"></param>
public sealed class PaymentMethodRepository(OrderHubDbContext context) : IPaymentMethodRepository
{
    public Task<PaymentMethod?> GetAsync(Guid tenantId, Guid establishmentId, Guid id, CancellationToken cancellationToken) => context.PaymentMethods.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Id == id, cancellationToken);

    public Task<bool> CodeExistsAsync(Guid tenantId, Guid establishmentId, string code, Guid? exceptId, CancellationToken cancellationToken) => context.PaymentMethods.AnyAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Code == code && (!exceptId.HasValue || x.Id != exceptId), cancellationToken);

    public async Task AddAsync(PaymentMethod method, CancellationToken cancellationToken)
    { await context.PaymentMethods.AddAsync(method, cancellationToken); await SaveChangesAsync(cancellationToken); }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    { try { await context.SaveChangesAsync(cancellationToken); } catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }) { throw new ConflictException("Payment-method code already exists."); } }
}

public sealed class PaymentRepository(OrderHubDbContext context) : IPaymentRepository
{
    public Task<Payment?> GetAsync(Guid tenantId, Guid establishmentId, Guid id, CancellationToken cancellationToken) => context.Payments.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Id == id, cancellationToken);

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken)
    { await context.Payments.AddAsync(payment, cancellationToken); await SaveChangesAsync(cancellationToken); }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    { try { await context.SaveChangesAsync(cancellationToken); } catch (DbUpdateConcurrencyException) { throw new ConflictException("Payment was changed concurrently."); } catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }) { throw new ConflictException("Payment confirmation was already processed."); } catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.CheckViolation }) { throw new ConflictException("Payment data violates order integrity."); } }
}

public sealed class PaymentIdempotencyRepository(OrderHubDbContext context) : IPaymentIdempotencyRepository
{
    public Task<PaymentIdempotency?> FindAsync(Guid tenantId, Guid establishmentId, string key, CancellationToken cancellationToken) => context.PaymentIdempotencies.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Key == key, cancellationToken);

    public async Task AddAsync(PaymentIdempotency idempotency, CancellationToken cancellationToken) => await context.PaymentIdempotencies.AddAsync(idempotency, cancellationToken);
}