using Microsoft.EntityFrameworkCore;
using OrderHub.Application.Abstractions.Identity;
using OrderHub.Domain.Identity;

namespace OrderHub.Infrastructure.Persistence.Write.Repositories;

public sealed class AdministrativeUserRepository(OrderHubDbContext context) : IAdministrativeUserRepository
{
    public Task<bool> EmailExistsAsync(Guid tenantId, string normalizedEmail, CancellationToken cancellationToken) =>
        context.AdministrativeUsers.AnyAsync(user => user.TenantId == tenantId && user.NormalizedEmail == normalizedEmail, cancellationToken);
    public async Task AddAsync(AdministrativeUser user, CancellationToken cancellationToken)
    { await context.AdministrativeUsers.AddAsync(user, cancellationToken); await context.SaveChangesAsync(cancellationToken); }
}
