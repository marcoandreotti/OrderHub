using OrderHub.Domain.Tenancy;

namespace OrderHub.Application.Abstractions.Tenancy;

public interface IEstablishmentRepository
{
    Task<bool> SlugExistsAsync(string normalizedSlug, CancellationToken cancellationToken);
    Task AddAsync(Establishment establishment, CancellationToken cancellationToken);
}
