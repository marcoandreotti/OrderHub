using OrderHub.Application.Identity.CreateAdministrativeUser;
using OrderHub.Application.Identity.Management;
using OrderHub.Domain.Identity;

namespace OrderHub.Application.Tests.Identity;

public sealed class AdministrativeUserManagementValidationTests
{
    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [InlineData(1000001, 20)]
    public void Rejects_invalid_pagination(int page, int size) =>
        Assert.False(new SearchAdministrativeUsersQueryValidator().Validate(new SearchAdministrativeUsersQuery(Guid.NewGuid(), Page: page, PageSize: size)).IsValid);

    [Fact]
    public void Validates_ids_roles_profile_and_creation()
    {
        var unit = Guid.NewGuid(); var user = Guid.NewGuid();
        Assert.False(new SetAdministrativeUserRoleCommandValidator().Validate(new SetAdministrativeUserRoleCommand(unit, user, (AdministrativeRole)99, true)).IsValid);
        Assert.False(new SetAdministrativeUserActiveCommandValidator().Validate(new SetAdministrativeUserActiveCommand(unit, Guid.Empty, false)).IsValid);
        Assert.False(new SetAdministrativeUserAccessCommandValidator().Validate(new SetAdministrativeUserAccessCommand(Guid.Empty, user, true)).IsValid);
        Assert.False(new UpdateAdministrativeUserCommandValidator().Validate(new UpdateAdministrativeUserCommand(unit, user, " ")).IsValid);
        Assert.False(new CreateAdministrativeUserCommandValidator().Validate(new CreateAdministrativeUserCommand("Name", "email@example.test", "secure-password", AdministrativeRole.Admin)).IsValid);
        Assert.True(new CreateAdministrativeUserCommandValidator().Validate(new CreateAdministrativeUserCommand("Name", "email@example.test", "secure-password", AdministrativeRole.Admin, unit)).IsValid);
    }
}
