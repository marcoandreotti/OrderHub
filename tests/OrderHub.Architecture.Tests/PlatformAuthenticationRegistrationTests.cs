using Microsoft.Extensions.DependencyInjection;
using OrderHub.Application;
using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Identity.Authentication;
using OrderHub.Infrastructure;

namespace OrderHub.Architecture.Tests;

public sealed class PlatformAuthenticationRegistrationTests
{
    [Fact]
    public void Platform_feature_replaces_existing_authentication_handlers()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddPlatformProvisioning();

        Assert.Single(services, x => x.ServiceType ==
            typeof(ICommandHandler<CompleteAuthenticationCommand, AuthenticationTokens>));
        Assert.Single(services, x => x.ServiceType ==
            typeof(ICommandHandler<RefreshAuthenticationCommand, AuthenticationTokens>));
        Assert.Single(services, x => x.ServiceType ==
            typeof(ICommandHandler<ChangeTemporaryPasswordCommand>));
    }
}
