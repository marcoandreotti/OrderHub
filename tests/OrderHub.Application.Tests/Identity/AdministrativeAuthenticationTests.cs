using OrderHub.Application.Abstractions.Identity;
using OrderHub.Application.Exceptions;
using OrderHub.Application.Identity.Authentication;
using OrderHub.Domain.Identity;

namespace OrderHub.Application.Tests.Identity;

public sealed class AdministrativeAuthenticationTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 3, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Platform_login_creates_restricted_session_until_temporary_password_is_changed()
    {
        var repository = new AuthenticationRepositoryFake();
        var secrets = new AuthenticationSecretProtectorFake();
        var passwords = new PasswordHasherFake();
        var sender = new AuthenticationCodeSenderFake();
        var clock = new FixedTimeProvider(Now);
        var options = new AuthenticationOptions { PlatformCode = "ORDERHUB" };
        var platformUser = PlatformUser.Bootstrap(new Email("root@orderhub.test"), passwords.Hash("temporary-password"), Now);
        repository.PlatformUsers.Add(platformUser);

        var begin = new BeginAuthenticationCommandHandler(repository, passwords, secrets, sender, clock, options);
        var challenge = await begin.HandleAsync(
            new BeginAuthenticationCommand("orderhub", "root@orderhub.test", "temporary-password", "127.0.0.1"),
            CancellationToken.None);

        Assert.Equal("root@orderhub.test", sender.Email);
        Assert.Equal("123456", sender.Code);

        var complete = new CompleteAuthenticationCommandHandler(repository, secrets, clock, options);
        var tokens = await complete.HandleAsync(
            new CompleteAuthenticationCommand(challenge.ChallengeId, sender.Code!, "127.0.0.1"),
            CancellationToken.None);

        Assert.True(tokens.PasswordChangeRequired);
        var session = Assert.Single(repository.Sessions);
        Assert.Equal(AuthenticationIdentityType.PlatformUser, session.IdentityType);
        Assert.Null(session.TenantId);

        var restrictedResolver = new SessionResolverFake(platformUser.Id, passwordChangeRequired: true);
        var createUser = new CreatePlatformUserCommandHandler(restrictedResolver, repository, passwords, clock);
        await Assert.ThrowsAsync<ForbiddenException>(() => createUser.HandleAsync(
            new CreatePlatformUserCommand(tokens.AccessToken, "peer@orderhub.test", "another-password"),
            CancellationToken.None));

        var changePassword = new ChangeTemporaryPasswordCommandHandler(repository, secrets, passwords, clock);
        await changePassword.HandleAsync(
            new ChangeTemporaryPasswordCommand(tokens.AccessToken, "temporary-password", "definitive-password"),
            CancellationToken.None);

        Assert.False(platformUser.PasswordChangeRequired);
        Assert.False(session.IsAccessValid(Now));
    }

    [Fact]
    public async Task Fully_authenticated_superuser_can_create_a_peer_but_tenant_user_cannot()
    {
        var repository = new AuthenticationRepositoryFake();
        var passwords = new PasswordHasherFake();
        var clock = new FixedTimeProvider(Now);
        var actorId = Guid.NewGuid();
        var platformResolver = new SessionResolverFake(actorId, passwordChangeRequired: false);
        var handler = new CreatePlatformUserCommandHandler(platformResolver, repository, passwords, clock);

        var createdId = await handler.HandleAsync(
            new CreatePlatformUserCommand("access", "peer@orderhub.test", "temporary-password"),
            CancellationToken.None);

        Assert.Equal(createdId, Assert.Single(repository.PlatformUsers).Id);
        var tenantResolver = new SessionResolverFake(actorId, passwordChangeRequired: false, AuthenticationIdentityType.AdministrativeUser);
        var forbiddenHandler = new CreatePlatformUserCommandHandler(tenantResolver, repository, passwords, clock);
        await Assert.ThrowsAsync<ForbiddenException>(() => forbiddenHandler.HandleAsync(
            new CreatePlatformUserCommand("access", "other@orderhub.test", "temporary-password"),
            CancellationToken.None));
    }

    [Fact]
    public async Task Same_email_in_two_tenants_is_resolved_only_by_the_matching_public_code()
    {
        var firstTenantId = Guid.NewGuid();
        var secondTenantId = Guid.NewGuid();
        var passwords = new PasswordHasherFake();
        var repository = new AuthenticationRepositoryFake();
        repository.AdministrativeUsers["FIRST"] = AdministrativeUser.Create(firstTenantId, "First", new Email("admin@example.test"), passwords.Hash("first-password"), AdministrativeRole.Owner, Now);
        repository.AdministrativeUsers["SECOND"] = AdministrativeUser.Create(secondTenantId, "Second", new Email("admin@example.test"), passwords.Hash("second-password"), AdministrativeRole.Owner, Now);
        var handler = new BeginAuthenticationCommandHandler(repository, passwords, new AuthenticationSecretProtectorFake(), new AuthenticationCodeSenderFake(), new FixedTimeProvider(Now), new AuthenticationOptions());

        var result = await handler.HandleAsync(
            new BeginAuthenticationCommand(" second ", "admin@example.test", "second-password", "127.0.0.1"),
            CancellationToken.None);

        Assert.Equal(secondTenantId, Assert.Single(repository.Challenges).TenantId);
        await Assert.ThrowsAsync<UnauthorizedException>(() => handler.HandleAsync(
            new BeginAuthenticationCommand("FIRST", "admin@example.test", "second-password", "127.0.0.2"),
            CancellationToken.None));
        Assert.NotEqual(Guid.Empty, result.ChallengeId);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class PasswordHasherFake : IPasswordHasher
    {
        public string Hash(string password) => $"password:{password}";
        public bool Verify(string passwordHash, string password) => passwordHash == Hash(password);
    }

    private sealed class AuthenticationSecretProtectorFake : IAuthenticationSecretProtector
    {
        private int tokenNumber;
        public string GenerateCode() => "123456";
        public string GenerateToken() => $"token-{Interlocked.Increment(ref tokenNumber)}";
        public string Hash(string value) => $"secret:{value}";
        public bool Verify(string hash, string value) => hash == Hash(value);
    }

    private sealed class AuthenticationCodeSenderFake : IAuthenticationCodeSender
    {
        public string? Email { get; private set; }
        public string? Code { get; private set; }
        public Task SendAsync(string email, string code, DateTimeOffset expiresAt, CancellationToken cancellationToken)
        {
            Email = email;
            Code = code;
            return Task.CompletedTask;
        }
    }

    private sealed class SessionResolverFake(Guid identityId, bool passwordChangeRequired, AuthenticationIdentityType type = AuthenticationIdentityType.PlatformUser) : IAuthenticationSessionResolver
    {
        public Task<AuthenticatedIdentity?> ResolveAsync(string accessToken, CancellationToken cancellationToken) =>
            Task.FromResult<AuthenticatedIdentity?>(new(Guid.NewGuid(), type, identityId, null, [], [], passwordChangeRequired));
    }

    private sealed class AuthenticationRepositoryFake : IAuthenticationRepository
    {
        public List<PlatformUser> PlatformUsers { get; } = [];
        public Dictionary<string, AdministrativeUser> AdministrativeUsers { get; } = new(StringComparer.OrdinalIgnoreCase);
        public List<AuthenticationChallenge> Challenges { get; } = [];
        public List<AdministrativeSession> Sessions { get; } = [];

        public Task<(Guid TenantId, AdministrativeUser User)?> FindAdministrativeUserAsync(string tenantCode, string normalizedEmail, CancellationToken cancellationToken)
        {
            var code = tenantCode.Trim();
            var match = AdministrativeUsers.GetValueOrDefault(code);
            return Task.FromResult<(Guid, AdministrativeUser)?>(match is not null && match.NormalizedEmail == normalizedEmail ? (match.TenantId, match) : null);
        }

        public Task<PlatformUser?> FindPlatformUserAsync(string normalizedEmail, CancellationToken cancellationToken) => Task.FromResult(PlatformUsers.SingleOrDefault(x => x.NormalizedEmail == normalizedEmail));
        public Task<PlatformUser?> GetPlatformUserAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(PlatformUsers.SingleOrDefault(x => x.Id == id));
        public Task<bool> AnyPlatformUserAsync(CancellationToken cancellationToken) => Task.FromResult(PlatformUsers.Count != 0);
        public Task<int> CountActivePlatformUsersAsync(CancellationToken cancellationToken) => Task.FromResult(PlatformUsers.Count(x => x.IsActive));
        public Task AddPlatformUserAsync(PlatformUser user, CancellationToken cancellationToken) { PlatformUsers.Add(user); return Task.CompletedTask; }
        public Task<AuthenticationChallenge?> GetChallengeAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Challenges.SingleOrDefault(x => x.Id == id));
        public Task AddChallengeAsync(AuthenticationChallenge challenge, CancellationToken cancellationToken) { Challenges.Add(challenge); return Task.CompletedTask; }
        public Task<int> CountRecentChallengesAsync(string originHash, DateTimeOffset since, CancellationToken cancellationToken) => Task.FromResult(Challenges.Count(x => x.OriginHash == originHash && x.CreatedAt >= since));
        public Task<AdministrativeSession?> FindSessionByAccessHashAsync(string hash, CancellationToken cancellationToken) => Task.FromResult(Sessions.SingleOrDefault(x => x.AccessTokenHash == hash));
        public Task<AdministrativeSession?> FindSessionByRefreshHashAsync(string hash, CancellationToken cancellationToken) => Task.FromResult(Sessions.SingleOrDefault(x => x.RefreshTokenHash == hash));
        public Task AddSessionAsync(AdministrativeSession session, CancellationToken cancellationToken) { Sessions.Add(session); return Task.CompletedTask; }
        public Task RevokeFamilyAsync(Guid familyId, DateTimeOffset now, CancellationToken cancellationToken) { foreach (var session in Sessions.Where(x => x.FamilyId == familyId)) session.Revoke(now); return Task.CompletedTask; }
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
