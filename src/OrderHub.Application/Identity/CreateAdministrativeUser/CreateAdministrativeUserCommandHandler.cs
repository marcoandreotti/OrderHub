using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Identity;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Identity;

namespace OrderHub.Application.Identity.CreateAdministrativeUser;

public sealed class CreateAdministrativeUserCommandHandler(
    ITenantContext tenantContext,
    IAdministrativeUserRepository repository,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider) : ICommandHandler<CreateAdministrativeUserCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateAdministrativeUserCommand command, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.GetRequiredTenantId();
        var email = new Email(command.Email);
        if (await repository.EmailExistsAsync(tenantId, email.NormalizedValue, cancellationToken))
        {
            throw new ConflictException("The administrative user email is already in use in this tenant.");
        }

        var user = AdministrativeUser.Create(
            tenantId,
            command.Name,
            email,
            passwordHasher.Hash(command.Password),
            command.InitialRole,
            timeProvider.GetUtcNow());
        await repository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}
