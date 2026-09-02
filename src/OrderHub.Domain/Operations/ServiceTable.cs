using System.Security.Cryptography;
using OrderHub.Domain.Exceptions;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Domain.Operations;

public sealed class ServiceTable : IEstablishmentScopedEntity
{
    private ServiceTable() { }
    private ServiceTable(Guid tenantId, Guid establishmentId, string code, string? description)
    {
        if (tenantId == Guid.Empty || establishmentId == Guid.Empty) throw new DomainException("Tenant and establishment are required.");
        Id = Guid.NewGuid(); TenantId = tenantId; EstablishmentId = establishmentId;
        Code = NormalizeCode(code); Description = NormalizeDescription(description); QrCodeToken = GenerateToken(); IsActive = true;
    }
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid EstablishmentId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string QrCodeToken { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public static ServiceTable Create(Guid tenantId, Guid establishmentId, string code, string? description = null) => new(tenantId, establishmentId, code, description);
    public void RevokeToken() => QrCodeToken = GenerateToken();
    public void Deactivate() => IsActive = false;
    private static string NormalizeCode(string code)
    { var value = code.Trim().ToUpperInvariant(); if (value.Length is < 1 or > 30) throw new DomainException("Table code must contain 1 to 30 characters."); return value; }
    private static string? NormalizeDescription(string? value)
    { if (string.IsNullOrWhiteSpace(value)) return null; var result = value.Trim(); if (result.Length > 100) throw new DomainException("Table description is too long."); return result; }
    private static string GenerateToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
}
