using Microsoft.EntityFrameworkCore;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Domain.Tenancy;

namespace OrderHub.Infrastructure.Persistence.Write.Repositories;

/// <summary>
/// Representa um repositório de estabelecimentos que fornece métodos para verificar a existência de slugs e adicionar novos estabelecimentos.
/// </summary>

public sealed class EstablishmentRepository(OrderHubDbContext context) : IEstablishmentRepository
{
    public Task<bool> SlugExistsAsync(string normalizedSlug, CancellationToken cancellationToken) =>
        context.Establishments.AnyAsync(
            establishment => establishment.Slug == new Slug(normalizedSlug),
            cancellationToken);

    public async Task AddAsync(Establishment establishment, CancellationToken cancellationToken)
    {
        await context.Establishments.AddAsync(establishment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}