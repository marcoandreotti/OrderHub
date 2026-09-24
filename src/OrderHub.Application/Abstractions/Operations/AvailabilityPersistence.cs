using OrderHub.Domain.Operations;
using OrderHub.Domain.Ordering;

namespace OrderHub.Application.Abstractions.Operations;

public interface IAvailabilityRepository
{
    Task<IReadOnlyList<ServiceScheduleException>> GetExceptionsAsync(Guid tenantId, Guid establishmentId, CancellationToken cancellationToken);
    void ReplaceExceptions(IReadOnlyCollection<ServiceScheduleException> previous, IReadOnlyCollection<ServiceScheduleException> replacement);
    Task<ServicePause?> GetActivePauseAsync(Guid tenantId, Guid establishmentId, OrderServiceType serviceType, DateTimeOffset instant, CancellationToken cancellationToken);
    void AddPause(ServicePause pause);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IAvailabilityReadGateway
{
    Task<AvailabilityDecision> EvaluateAsync(Guid tenantId, Guid establishmentId, OrderServiceType serviceType, DateTimeOffset instant, CancellationToken cancellationToken);
}
