namespace OrderHub.Domain.SharedKernel;

public interface ITenantScopedEntity
{
    Guid TenantId { get; }
}
