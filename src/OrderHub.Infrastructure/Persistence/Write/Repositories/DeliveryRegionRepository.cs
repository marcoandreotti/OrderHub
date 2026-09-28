using Microsoft.EntityFrameworkCore;
using OrderHub.Application.Abstractions.Delivery;
using OrderHub.Domain.Delivery;
using OrderHub.Infrastructure.Persistence.Write;

namespace OrderHub.Infrastructure.Persistence.Write.Repositories;

internal sealed class DeliveryRegionRepository(OrderHubDbContext db) : IDeliveryRegionRepository
{
    public Task<DeliveryRegion?> GetAsync(Guid tenantId, Guid establishmentId, Guid id, CancellationToken cancellationToken) =>
        db.DeliveryRegions.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.EstablishmentId == establishmentId && x.Id == id, cancellationToken);

    public async Task AddAsync(DeliveryRegion region, CancellationToken cancellationToken) =>
        await db.DeliveryRegions.AddAsync(region, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
