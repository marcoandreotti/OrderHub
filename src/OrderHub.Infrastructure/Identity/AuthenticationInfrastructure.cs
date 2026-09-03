using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using OrderHub.Application.Abstractions.Identity;
using OrderHub.Domain.Identity;
using OrderHub.Domain.Tenancy;
using OrderHub.Infrastructure.Persistence.Write;

namespace OrderHub.Infrastructure.Identity;

public sealed class AuthenticationSecretProtector : IAuthenticationSecretProtector
{
    public string GenerateCode() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    public string GenerateToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    public string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    public bool Verify(string hash, string value) => CryptographicOperations.FixedTimeEquals(Convert.FromHexString(hash), Convert.FromHexString(Hash(value)));
}

public sealed class AuthenticationEmailOptions
{
    public const string SectionName = "AuthenticationEmail";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}

public sealed class SmtpAuthenticationCodeSender(IOptions<AuthenticationEmailOptions> options, ILogger<SmtpAuthenticationCodeSender> logger) : IAuthenticationCodeSender
{
    public async Task SendAsync(string email, string code, DateTimeOffset expiresAt, CancellationToken ct)
    {
        var value = options.Value;
        if (string.IsNullOrWhiteSpace(value.Host) || string.IsNullOrWhiteSpace(value.FromAddress)) throw new InvalidOperationException("Authentication e-mail delivery is not configured.");
        using var message = new MailMessage(value.FromAddress, email, "Código de acesso OrderHub", $"Seu código de acesso é {code}. Ele expira em {expiresAt:O}.");
        using var client = new SmtpClient(value.Host, value.Port) { EnableSsl = value.EnableSsl };
        if (!string.IsNullOrWhiteSpace(value.Username)) client.Credentials = new NetworkCredential(value.Username, value.Password);
        logger.LogInformation("Sending authentication code to e-mail domain {EmailDomain}", email[(email.LastIndexOf('@') + 1)..]);
        await client.SendMailAsync(message, ct);
    }
}

public sealed class AuthenticationRepository(OrderHubDbContext db) : IAuthenticationRepository
{
    public async Task<(Guid TenantId, AdministrativeUser User)?> FindAdministrativeUserAsync(string tenantCode, string normalizedEmail, CancellationToken ct)
    { var code = Tenant.NormalizePublicCode(tenantCode); var user = await db.AdministrativeUsers.Include(x => x.RoleMemberships).Include(x => x.EstablishmentAccesses).SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail && db.Tenants.Any(t => t.Id == x.TenantId && t.PublicCode == code && t.IsActive), ct); return user is null ? null : (user.TenantId, user); }
    public Task<PlatformUser?> FindPlatformUserAsync(string email, CancellationToken ct) => db.PlatformUsers.SingleOrDefaultAsync(x => x.NormalizedEmail == email, ct);
    public Task<PlatformUser?> GetPlatformUserAsync(Guid id, CancellationToken ct) => db.PlatformUsers.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> AnyPlatformUserAsync(CancellationToken ct) => db.PlatformUsers.AnyAsync(ct);
    public Task<int> CountActivePlatformUsersAsync(CancellationToken ct) => db.PlatformUsers.CountAsync(x => x.IsActive, ct);
    public async Task AddPlatformUserAsync(PlatformUser user, CancellationToken ct) => await db.PlatformUsers.AddAsync(user, ct);
    public Task<AuthenticationChallenge?> GetChallengeAsync(Guid id, CancellationToken ct) => db.AuthenticationChallenges.SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task AddChallengeAsync(AuthenticationChallenge challenge, CancellationToken ct) => await db.AuthenticationChallenges.AddAsync(challenge, ct);
    public Task<int> CountRecentChallengesAsync(string originHash, DateTimeOffset since, CancellationToken ct) => db.AuthenticationChallenges.CountAsync(x => x.OriginHash == originHash && x.CreatedAt >= since, ct);
    public Task<AdministrativeSession?> FindSessionByAccessHashAsync(string hash, CancellationToken ct) => db.AdministrativeSessions.SingleOrDefaultAsync(x => x.AccessTokenHash == hash, ct);
    public Task<AdministrativeSession?> FindSessionByRefreshHashAsync(string hash, CancellationToken ct) => db.AdministrativeSessions.SingleOrDefaultAsync(x => x.RefreshTokenHash == hash, ct);
    public async Task AddSessionAsync(AdministrativeSession session, CancellationToken ct) => await db.AdministrativeSessions.AddAsync(session, ct);
    public async Task RevokeFamilyAsync(Guid familyId, DateTimeOffset now, CancellationToken ct) { var sessions = await db.AdministrativeSessions.Where(x => x.FamilyId == familyId && x.RevokedAt == null).ToListAsync(ct); foreach (var s in sessions) s.Revoke(now); }
    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}

public sealed class AuthenticationSessionResolver(OrderHubDbContext db, IAuthenticationSecretProtector secrets, TimeProvider clock) : IAuthenticationSessionResolver
{
    public async Task<AuthenticatedIdentity?> ResolveAsync(string token, CancellationToken ct)
    {
        var hash = secrets.Hash(token); var s = await db.AdministrativeSessions.AsNoTracking().SingleOrDefaultAsync(x => x.AccessTokenHash == hash, ct); if (s is null || !s.IsAccessValid(clock.GetUtcNow())) return null;
        if (s.IdentityType == AuthenticationIdentityType.PlatformUser) { var p = await db.PlatformUsers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == s.IdentityId && x.IsActive, ct); return p is null ? null : new(s.Id, s.IdentityType, s.IdentityId, null, [], [], p.PasswordChangeRequired); }
        var u = await db.AdministrativeUsers.AsNoTracking().Include(x => x.RoleMemberships).Include(x => x.EstablishmentAccesses).SingleOrDefaultAsync(x => x.Id == s.IdentityId && x.TenantId == s.TenantId && x.IsActive, ct); return u is null ? null : new(s.Id, s.IdentityType, s.IdentityId, s.TenantId, u.RoleMemberships.Select(x => x.Role).ToArray(), u.EstablishmentAccesses.Where(x => x.IsActive).Select(x => x.EstablishmentId).ToArray(), false);
    }
}
