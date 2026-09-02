namespace OrderHub.Application.Abstractions.Tenancy;

public interface ITenantContext
{
    bool HasTenant { get; }
    Guid TenantId { get; }
    bool HasUser { get; }
    Guid UserId { get; }
    Guid GetRequiredTenantId();
    Guid GetRequiredUserId();
}
