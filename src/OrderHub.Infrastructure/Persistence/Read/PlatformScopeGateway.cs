using Microsoft.EntityFrameworkCore;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Infrastructure.Persistence.Write;

namespace OrderHub.Infrastructure.Persistence.Read;

public sealed class PlatformScopeGateway(OrderHubDbContext db):IPlatformScopeGateway
{
    public Task<Guid?> FindTenantIdAsync(Guid establishmentId,CancellationToken ct)=>db.Establishments.AsNoTracking().Where(x=>x.Id==establishmentId).Select(x=>(Guid?)x.TenantId).SingleOrDefaultAsync(ct);
}
