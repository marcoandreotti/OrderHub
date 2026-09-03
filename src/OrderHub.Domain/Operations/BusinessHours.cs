using OrderHub.Domain.Exceptions;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Domain.Operations;

/// <summary>
/// Representa o intervalo de atendimento de um estabelecimento em um determinado dia da semana, com horário de abertura e fechamento, e status de atividade.
/// </summary>
public sealed class BusinessHours : IEstablishmentScopedEntity
{
    private BusinessHours()
    { }

    private BusinessHours(Guid tenantId, Guid establishmentId, DayOfWeek dayOfWeek, TimeOnly opensAt, TimeOnly closesAt)
    {
        if (tenantId == Guid.Empty || establishmentId == Guid.Empty || closesAt <= opensAt) throw new DomainException("Business-hours interval must close after it opens on the same day.");
        Id = Guid.NewGuid(); TenantId = tenantId; EstablishmentId = establishmentId; DayOfWeek = dayOfWeek; OpensAt = opensAt; ClosesAt = closesAt; IsActive = true;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly OpensAt { get; private set; }
    public TimeOnly ClosesAt { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>Cria um intervalo de atendimento válido para um estabelecimento.</summary>
    public static BusinessHours Create(Guid tenantId, Guid establishmentId, DayOfWeek dayOfWeek, TimeOnly opensAt, TimeOnly closesAt) => new(tenantId, establishmentId, dayOfWeek, opensAt, closesAt);

    /// <summary>Indica se o dia e horário pertencem ao intervalo ativo de atendimento.</summary>
    public bool Contains(DayOfWeek day, TimeOnly time) => IsActive && day == DayOfWeek && time >= OpensAt && time < ClosesAt;

    public void Deactivate() => IsActive = false;
}