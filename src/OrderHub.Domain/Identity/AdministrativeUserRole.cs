namespace OrderHub.Domain.Identity;

public sealed class AdministrativeUserRole
{
    private AdministrativeUserRole() { }
    internal AdministrativeUserRole(Guid userId, AdministrativeRole role) { UserId = userId; Role = role; }
    public Guid UserId { get; private set; }
    public AdministrativeRole Role { get; private set; }
}
