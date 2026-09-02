using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Application.Exceptions;

namespace OrderHub.Application.Tenancy;

public sealed class EstablishmentScopeResolver(
    ITenantContext tenantContext,
    IEstablishmentAccessGateway accessGateway)
{
    private OperationalScope? resolvedScope;

    /// <summary>Resolve o escopo operacional autenticado e impede troca de estabelecimento durante a requisição.</summary>
    public async Task<OperationalScope> ResolveAsync(
        Guid selectedEstablishmentId,
        CancellationToken cancellationToken)
    {
        if (selectedEstablishmentId == Guid.Empty)
        {
            throw new ForbiddenException("An authorized establishment context is required.");
        }

        if (resolvedScope is { } existing)
        {
            if (existing.EstablishmentId != selectedEstablishmentId)
            {
                throw new ForbiddenException("The establishment context cannot change during a request.");
            }

            return existing;
        }

        var tenantId = tenantContext.GetRequiredTenantId();
        var userId = tenantContext.GetRequiredUserId();
        var hasAccess = await accessGateway.HasActiveAccessAsync(
            tenantId,
            userId,
            selectedEstablishmentId,
            cancellationToken);

        if (!hasAccess)
        {
            throw new ForbiddenException("An authorized establishment context is required.");
        }

        resolvedScope = new OperationalScope(tenantId, userId, selectedEstablishmentId);
        return resolvedScope;
    }
}
