using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderHub.Application.Abstractions.Identity;
using OrderHub.Application.Identity.Authentication;
using OrderHub.Contracts.Authentication;
using OrderHub.Domain.Identity;

namespace OrderHub.Integration.Tests;

public sealed class AuthenticationContextApiTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("expired")]
    public async Task Missing_or_expired_session_is_rejected(string? token)
    {
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        if (token is not null) client.DefaultRequestHeaders.Add("Cookie", $"oh_access={token}");
        using var response = await client.GetAsync("/api/auth/context");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Restricted_session_can_read_context_but_not_operational_data()
    {
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", "oh_access=restricted");
        using var response = await client.GetAsync("/api/auth/context");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthenticationContextResponse>();
        Assert.True(result!.PasswordChangeRequired);
        Assert.Empty(result.Capabilities);
        Assert.Empty(result.Establishments);
        Assert.True(response.Headers.CacheControl!.NoStore);
        using var forbidden = await client.GetAsync($"/api/admin/establishments/{Guid.NewGuid()}/customers");
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }

    [Fact]
    public async Task Context_exposes_server_capabilities_without_tokens_or_internal_claims()
    {
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", "oh_access=active");
        using var response = await client.GetAsync("/api/auth/context");
        var result = await response.Content.ReadFromJsonAsync<AuthenticationContextResponse>();
        Assert.Contains("management", result!.Capabilities);
        Assert.DoesNotContain("administration", result.Capabilities);
        Assert.Single(result.Establishments);
        var json = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("sessionId", json);
        Assert.DoesNotContain("accessToken", json);
        Assert.DoesNotContain("tenantId", json);
    }

    [Fact]
    public async Task Invalid_login_attempts_are_rate_limited_before_database_access()
    {
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        for (var index = 0; index < 20; index++)
        {
            using var response = await client.PostAsJsonAsync("/api/auth/begin", new BeginAuthenticationRequest("", "", ""));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        using var limited = await client.PostAsJsonAsync("/api/auth/begin", new BeginAuthenticationRequest("", "", ""));
        Assert.Equal(HttpStatusCode.TooManyRequests, limited.StatusCode);
    }

    private sealed class Factory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IAuthenticationSessionResolver>();
                services.RemoveAll<IAuthenticationContextGateway>();
                services.AddSingleton<IAuthenticationSessionResolver, Resolver>();
                services.AddSingleton<IAuthenticationContextGateway, Gateway>();
            });
        }
    }

    private sealed class Resolver : IAuthenticationSessionResolver
    {
        public Task<AuthenticatedIdentity?> ResolveAsync(string token, CancellationToken ct) => Task.FromResult<AuthenticatedIdentity?>(token switch
        {
            "restricted" => new(Guid.NewGuid(), AuthenticationIdentityType.PlatformUser, Guid.NewGuid(), null, [], [], true),
            "active" => new(Guid.NewGuid(), AuthenticationIdentityType.AdministrativeUser, Guid.NewGuid(), Guid.NewGuid(), [AdministrativeRole.Manager], [], false),
            _ => null
        });
    }

    private sealed class Gateway : IAuthenticationContextGateway
    {
        public Task<IReadOnlyCollection<AuthenticationEstablishment>> GetEstablishmentsAsync(AuthenticatedIdentity identity, CancellationToken ct) =>
            Task.FromResult<IReadOnlyCollection<AuthenticationEstablishment>>([new(Guid.NewGuid(), "Unidade autorizada")]);
    }
}
