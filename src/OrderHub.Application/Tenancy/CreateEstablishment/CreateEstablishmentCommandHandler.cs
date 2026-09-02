using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Tenancy;

namespace OrderHub.Application.Tenancy.CreateEstablishment;

public sealed class CreateEstablishmentCommandHandler(
    ITenantContext tenantContext,
    IEstablishmentRepository repository,
    TimeProvider timeProvider) : ICommandHandler<CreateEstablishmentCommand, Guid>
{
    public async Task<Guid> HandleAsync(
        CreateEstablishmentCommand command,
        CancellationToken cancellationToken)
    {
        var slug = new Slug(command.Slug);
        if (await repository.SlugExistsAsync(slug.Value, cancellationToken))
        {
            throw new ConflictException("The establishment slug is already in use.");
        }

        var establishment = Establishment.Create(
            tenantContext.GetRequiredTenantId(),
            command.TradeName,
            slug,
            timeProvider.GetUtcNow());
        await repository.AddAsync(establishment, cancellationToken);
        return establishment.Id;
    }
}
