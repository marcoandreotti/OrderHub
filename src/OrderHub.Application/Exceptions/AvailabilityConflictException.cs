using OrderHub.Domain.Operations;

namespace OrderHub.Application.Exceptions;

public sealed class AvailabilityConflictException(
    string message,
    AvailabilityReason reason,
    DateTimeOffset? nextOpening = null,
    IReadOnlyCollection<Guid>? affectedOfferIds = null) : Exception(message)
{
    public AvailabilityReason Reason { get; } = reason;
    public DateTimeOffset? NextOpening { get; } = nextOpening;
    public IReadOnlyCollection<Guid> AffectedOfferIds { get; } = affectedOfferIds ?? [];
}
