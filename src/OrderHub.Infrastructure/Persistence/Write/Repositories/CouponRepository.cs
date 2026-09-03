using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderHub.Application.Abstractions.Promotions;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Promotions;

namespace OrderHub.Infrastructure.Persistence.Write.Repositories;

public sealed class CouponRepository(OrderHubDbContext context) : ICouponRepository
{
    public Task<Coupon?> GetAsync(Guid tenantId, Guid establishmentId, Guid id, CancellationToken cancellationToken) => context.Coupons.Include(x => x.Uses).SingleOrDefaultAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Id == id, cancellationToken);
    public Task<Coupon?> FindByCodeAsync(Guid tenantId, Guid establishmentId, string normalizedCode, CancellationToken cancellationToken) => context.Coupons.Include(x => x.Uses).SingleOrDefaultAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Code == normalizedCode, cancellationToken);
    public async Task AddAsync(Coupon coupon, CancellationToken cancellationToken) { await context.Coupons.AddAsync(coupon, cancellationToken); await SaveChangesAsync(cancellationToken); }
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try { await context.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException) { throw new ConflictException("Coupon was changed or its usage limit was reached by another operation."); }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }) { throw new ConflictException("Coupon code or use already exists in this establishment."); }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.CheckViolation }) { throw new ConflictException("Coupon data violates establishment integrity."); }
    }
}
