using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Application.Exceptions;

namespace OrderHub.Api.Tenancy;

internal sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    private const string TenantClaim = "tenant_id";
    private const string UserClaim = "sub";

    public bool HasTenant => TryGetTenantId(out _);
    public Guid TenantId => GetRequiredTenantId();
    public bool HasUser => TryGetUserId(out _);
    public Guid UserId => GetRequiredUserId();
    public bool IsPlatformUser => httpContextAccessor.HttpContext?.User.HasClaim("platform_user", "true") == true;

    public Guid GetRequiredTenantId()
    {
        if (TryGetTenantId(out var tenantId))
        {
            return tenantId;
        }

        throw new ForbiddenException("A valid tenant context is required.");
    }

    public Guid GetRequiredUserId()
    {
        if (TryGetUserId(out var userId))
        {
            return userId;
        }

        throw new ForbiddenException("A valid authenticated user context is required.");
    }

    private bool TryGetTenantId(out Guid tenantId)
    {
        var claimValue = httpContextAccessor.HttpContext?.User.FindFirst(TenantClaim)?.Value;
        return Guid.TryParse(claimValue, out tenantId) && tenantId != Guid.Empty;
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claimValue = httpContextAccessor.HttpContext?.User.FindFirst(UserClaim)?.Value;
        return Guid.TryParse(claimValue, out userId) && userId != Guid.Empty;
    }
}
