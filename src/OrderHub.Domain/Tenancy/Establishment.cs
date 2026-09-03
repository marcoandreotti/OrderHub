using OrderHub.Domain.Exceptions;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Domain.Tenancy;

/// <summary>
/// Representa um estabelecimento vinculado a um tenant, com informações como nome comercial, slug, tema visual e status de atividade.
/// </summary>
public sealed class Establishment : ITenantScopedEntity
{
    private Establishment()
    {
    }

    private Establishment(Guid id, Guid tenantId, string tradeName, Slug slug, DateTimeOffset createdAt)
    {
        if (tenantId == Guid.Empty)
        {
            throw new DomainException("Tenant is required.");
        }

        Id = id;
        TenantId = tenantId;
        SetTradeName(tradeName);
        Slug = slug;
        Theme = new EstablishmentTheme();
        IsActive = true;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string TradeName { get; private set; } = string.Empty;
    public Slug Slug { get; private set; } = null!;
    public EstablishmentTheme Theme { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Cria um estabelecimento ativo, vinculado obrigatoriamente a um tenant.</summary>
    public static Establishment Create(Guid tenantId, string tradeName, Slug slug, DateTimeOffset now) =>
        new(Guid.NewGuid(), tenantId, tradeName, slug, now);

    /// <summary>Substitui o identificador público do estabelecimento.</summary>
    public void ChangeSlug(Slug slug, DateTimeOffset now)
    {
        Slug = slug;
        UpdatedAt = now;
    }

    /// <summary>Atualiza a identidade visual pública do estabelecimento.</summary>
    public void ChangeTheme(EstablishmentTheme theme, DateTimeOffset now)
    {
        Theme = theme;
        UpdatedAt = now;
    }

    /// <summary>Obtém o tema público, desde que o estabelecimento esteja ativo.</summary>
    public EstablishmentTheme GetPublicTheme()
    {
        if (!IsActive)
        {
            throw new DomainException("Inactive establishments do not expose a public theme.");
        }

        return Theme;
    }

    /// <summary>Desativa o estabelecimento e impede a exposição de seus dados públicos.</summary>
    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        UpdatedAt = now;
    }

    private void SetTradeName(string tradeName)
    {
        var normalized = tradeName.Trim();
        if (normalized.Length is < 1 or > 150)
        {
            throw new DomainException("Establishment trade name must contain 1 to 150 characters.");
        }

        TradeName = normalized;
    }
}