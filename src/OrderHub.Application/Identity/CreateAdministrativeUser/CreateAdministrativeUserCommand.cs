using OrderHub.Application.Abstractions.Commands;
using OrderHub.Domain.Identity;

namespace OrderHub.Application.Identity.CreateAdministrativeUser;

public sealed record CreateAdministrativeUserCommand(
    string Name,
    string Email,
    string Password,
    AdministrativeRole InitialRole) : ICommand<Guid>;
