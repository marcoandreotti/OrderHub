using OrderHub.Domain.Identity;

namespace OrderHub.Application.Abstractions.Identity;

public interface IAdministrativeUserRepository
{
    Task<bool> EmailExistsAsync(Guid tenantId, string normalizedEmail, CancellationToken cancellationToken);
    Task AddAsync(AdministrativeUser user, CancellationToken cancellationToken);
}
