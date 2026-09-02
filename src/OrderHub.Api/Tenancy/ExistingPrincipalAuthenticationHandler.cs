using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace OrderHub.Api.Tenancy;

internal sealed class ExistingPrincipalAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "ExistingPrincipal";
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.User.Identity?.IsAuthenticated == true)
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, SchemeName)));
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
