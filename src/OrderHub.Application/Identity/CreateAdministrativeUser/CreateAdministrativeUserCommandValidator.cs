using FluentValidation;

namespace OrderHub.Application.Identity.CreateAdministrativeUser;

public sealed class CreateAdministrativeUserCommandValidator : AbstractValidator<CreateAdministrativeUserCommand>
{
    public CreateAdministrativeUserCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(150);
        RuleFor(command => command.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(command => command.Password).NotEmpty().MinimumLength(12).MaximumLength(200);
        RuleFor(command => command.InitialRole).IsInEnum();
    }
}
