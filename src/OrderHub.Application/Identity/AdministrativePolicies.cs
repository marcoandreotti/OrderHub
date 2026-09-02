using OrderHub.Domain.Identity;

namespace OrderHub.Application.Identity;

public static class AdministrativePolicies
{
    public const string Ownership = "ownership";
    public const string Administration = "administration";
    public const string Management = "management";
    public const string Attendance = "attendance";
    public const string Kitchen = "kitchen";
    public const string Delivery = "delivery";

    public static IReadOnlyDictionary<string, AdministrativeRole[]> RoleMap { get; } =
        new Dictionary<string, AdministrativeRole[]>(StringComparer.Ordinal)
        {
            [Ownership] = [AdministrativeRole.Owner],
            [Administration] = [AdministrativeRole.Owner, AdministrativeRole.Admin],
            [Management] = [AdministrativeRole.Owner, AdministrativeRole.Admin, AdministrativeRole.Manager],
            [Attendance] = [AdministrativeRole.Owner, AdministrativeRole.Admin, AdministrativeRole.Manager, AdministrativeRole.Attendant],
            [Kitchen] = [AdministrativeRole.Owner, AdministrativeRole.Admin, AdministrativeRole.Manager, AdministrativeRole.Kitchen],
            [Delivery] = [AdministrativeRole.Owner, AdministrativeRole.Admin, AdministrativeRole.Manager, AdministrativeRole.Delivery]
        };
}
