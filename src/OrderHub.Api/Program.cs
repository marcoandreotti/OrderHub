using OrderHub.Api.Middleware;
using OrderHub.Api.Tenancy;
using OrderHub.Application;
using OrderHub.Application.Abstractions.Tenancy;
using OrderHub.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OrderHub.Application.Identity;
using OrderHub.Api.Catalog;
using OrderHub.Api.PublicOrdering;
using OrderHub.Api.Administration;
using OrderHub.Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using OrderHub.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
var authentication = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = builder.Environment.IsEnvironment("Testing") ? ExistingPrincipalAuthenticationHandler.SchemeName : SessionAuthenticationHandler.SchemeName;
    options.DefaultChallengeScheme = options.DefaultAuthenticateScheme;
});
if (builder.Environment.IsEnvironment("Testing")) authentication.AddScheme<AuthenticationSchemeOptions, ExistingPrincipalAuthenticationHandler>(ExistingPrincipalAuthenticationHandler.SchemeName, _ => { });
else authentication.AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(SessionAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddProblemDetails();
builder.Services.AddAuthorization(options =>
{
    foreach (var (policyName, roles) in AdministrativePolicies.RoleMap)
    {
        options.AddPolicy(policyName, policy => policy.RequireAssertion(context => context.User.HasClaim("platform_user", "true") || roles.Any(role => context.User.IsInRole(role.ToString()))));
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "OrderHub API v1"));
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseMiddleware<AuthenticationSecurityMiddleware>();
app.UseAuthorization();
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
app.MapCatalogEndpoints();
app.MapPublicOrderingEndpoints();
app.MapAdministrationEndpoints();
app.MapAuthenticationEndpoints();

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/_test/tenant", (ITenantContext tenantContext) => Results.Ok(new
    {
        tenantId = tenantContext.GetRequiredTenantId()
    }));
}
else if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<PlatformBootstrapper>().InitializeAsync(CancellationToken.None);
}

app.Run();

public partial class Program;
