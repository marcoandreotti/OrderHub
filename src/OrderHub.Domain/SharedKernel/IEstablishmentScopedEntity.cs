namespace OrderHub.Domain.SharedKernel;

public interface IEstablishmentScopedEntity : ITenantScopedEntity
{
    Guid EstablishmentId { get; }
}
