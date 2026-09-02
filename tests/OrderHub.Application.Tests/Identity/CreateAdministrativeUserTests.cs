using OrderHub.Application.Abstractions.Identity;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Application.Exceptions;
using OrderHub.Application.Identity;
using OrderHub.Application.Identity.CreateAdministrativeUser;
using OrderHub.Domain.Identity;

namespace OrderHub.Application.Tests.Identity;

public sealed class CreateAdministrativeUserTests
{
    [Fact]
    public async Task Hashes_password_and_scopes_email_uniqueness_to_tenant()
    {
        var tenantId = Guid.NewGuid();
        var repository = new FakeRepository();
        var hasher = new FakeHasher();
        var handler = new CreateAdministrativeUserCommandHandler(
            new FakeTenantContext(tenantId), repository, hasher, TimeProvider.System);

        await handler.HandleAsync(
            new CreateAdministrativeUserCommand("Marco", "marco@example.com", "a-secure-password", AdministrativeRole.Admin),
            CancellationToken.None);

        Assert.Equal(tenantId, repository.TenantChecked);
        Assert.Equal("MARCO@EXAMPLE.COM", repository.EmailChecked);
        Assert.Equal("hashed:a-secure-password", repository.Added!.PasswordHash);
    }

    [Fact]
    public async Task Rejects_duplicate_email_in_same_tenant()
    {
        var repository = new FakeRepository { Exists = true };
        var handler = new CreateAdministrativeUserCommandHandler(
            new FakeTenantContext(Guid.NewGuid()), repository, new FakeHasher(), TimeProvider.System);

        await Assert.ThrowsAsync<ConflictException>(() => handler.HandleAsync(
            new CreateAdministrativeUserCommand("Marco", "marco@example.com", "a-secure-password", AdministrativeRole.Admin),
            CancellationToken.None));
    }

    [Fact]
    public void Policies_map_operational_capabilities_to_explicit_roles()
    {
        Assert.Contains(AdministrativeRole.Owner, AdministrativePolicies.RoleMap[AdministrativePolicies.Administration]);
        Assert.DoesNotContain(AdministrativeRole.Delivery, AdministrativePolicies.RoleMap[AdministrativePolicies.Kitchen]);
    }

    private sealed class FakeRepository : IAdministrativeUserRepository
    {
        public bool Exists { get; init; }
        public Guid TenantChecked { get; private set; }
        public string? EmailChecked { get; private set; }
        public AdministrativeUser? Added { get; private set; }

        public Task<bool> EmailExistsAsync(Guid tenantId, string normalizedEmail, CancellationToken cancellationToken)
        {
            TenantChecked = tenantId;
            EmailChecked = normalizedEmail;
            return Task.FromResult(Exists);
        }

        public Task AddAsync(AdministrativeUser user, CancellationToken cancellationToken)
        {
            Added = user;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeHasher : IPasswordHasher
    {
        public string Hash(string password) => $"hashed:{password}";
        public bool Verify(string passwordHash, string password) => passwordHash == Hash(password);
    }

    private sealed class FakeTenantContext(Guid tenantId) : ITenantContext
    {
        public bool HasTenant => true;
        public Guid TenantId => tenantId;
        public bool HasUser => true;
        public Guid UserId { get; } = Guid.NewGuid();
        public Guid GetRequiredTenantId() => tenantId;
        public Guid GetRequiredUserId() => UserId;
    }
}
