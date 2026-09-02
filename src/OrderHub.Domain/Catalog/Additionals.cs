using OrderHub.Domain.Exceptions;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Domain.Catalog;

public sealed class Additional : IEstablishmentScopedEntity
{
    private Additional() { }
    private Additional(Guid tenantId, Guid establishmentId, string name, Money price)
    { if (tenantId == Guid.Empty || establishmentId == Guid.Empty) throw new DomainException("Additional scope is required."); var value = name.Trim(); if (value.Length is < 1 or > 150) throw new DomainException("Additional name is invalid."); Id = Guid.NewGuid(); TenantId = tenantId; EstablishmentId = establishmentId; Name = value; Price = price; IsActive = true; }
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Money Price { get; private set; }
    public bool IsActive { get; private set; }
    /// <summary>Cria um adicional ativo no catálogo do estabelecimento.</summary>
    public static Additional Create(Guid tenantId, Guid establishmentId, string name, Money price) => new(tenantId, establishmentId, name, price);
    public void Update(string name, Money price)
    { var value = name.Trim(); if (value.Length is < 1 or > 150) throw new DomainException("Additional name is invalid."); Name = value; Price = price; }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

public sealed class AdditionalGroup : IEstablishmentScopedEntity
{
    private readonly List<AdditionalGroupItem> items = [];
    private AdditionalGroup() { }
    private AdditionalGroup(Guid tenantId, Guid establishmentId, string name, int minimum, int maximum)
    { if (tenantId == Guid.Empty || establishmentId == Guid.Empty || minimum < 0 || maximum < 1 || minimum > maximum) throw new DomainException("Additional-group selection range is invalid."); var value = name.Trim(); if (value.Length is < 1 or > 150) throw new DomainException("Additional-group name is invalid."); Id = Guid.NewGuid(); TenantId = tenantId; EstablishmentId = establishmentId; Name = value; MinimumSelection = minimum; MaximumSelection = maximum; IsActive = true; }
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int MinimumSelection { get; private set; }
    public int MaximumSelection { get; private set; }
    public bool IsRequired => MinimumSelection > 0;
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<AdditionalGroupItem> Items => items;
    /// <summary>Cria um grupo de adicionais com limites válidos de seleção.</summary>
    public static AdditionalGroup Create(Guid tenantId, Guid establishmentId, string name, int minimum, int maximum) => new(tenantId, establishmentId, name, minimum, maximum);
    public void Update(string name, int minimum, int maximum)
    { if (minimum < 0 || maximum < 1 || minimum > maximum) throw new DomainException("Additional-group selection range is invalid."); var value = name.Trim(); if (value.Length is < 1 or > 150) throw new DomainException("Additional-group name is invalid."); Name = value; MinimumSelection = minimum; MaximumSelection = maximum; }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    /// <summary>Inclui um adicional do mesmo estabelecimento sem criar vínculos duplicados.</summary>
    public void AddItem(Additional additional, int order)
    { if (additional.TenantId != TenantId || additional.EstablishmentId != EstablishmentId || order < 0) throw new DomainException("Additional must belong to the same establishment."); if (items.All(item => item.AdditionalId != additional.Id)) items.Add(new AdditionalGroupItem(TenantId, EstablishmentId, Id, additional.Id, order)); }
    public void RemoveItem(Guid additionalId) => items.RemoveAll(item => item.AdditionalId == additionalId);
    /// <summary>Substitui integralmente os adicionais associados ao grupo.</summary>
    public void ReplaceItems(IEnumerable<(Additional Additional, int Order)> replacements)
    { items.Clear(); foreach (var item in replacements) AddItem(item.Additional, item.Order); }
    /// <summary>Valida se a quantidade selecionada respeita os limites configurados.</summary>
    public void ValidateSelection(int selectedCount) { if (selectedCount < MinimumSelection || selectedCount > MaximumSelection) throw new DomainException("Additional selection is outside the allowed range."); }
}

public sealed class AdditionalGroupItem
{
    private AdditionalGroupItem() { }
    internal AdditionalGroupItem(Guid tenantId, Guid establishmentId, Guid groupId, Guid additionalId, int order) { TenantId = tenantId; EstablishmentId = establishmentId; GroupId = groupId; AdditionalId = additionalId; Order = order; }
    public Guid TenantId { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid AdditionalId { get; private set; }
    public int Order { get; private set; }
}
