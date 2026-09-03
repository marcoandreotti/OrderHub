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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
builder.Services.AddAuthentication(ExistingPrincipalAuthenticationHandler.SchemeName)
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, ExistingPrincipalAuthenticationHandler>(ExistingPrincipalAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddProblemDetails();
builder.Services.AddAuthorization(options =>
{
    foreach (var (policyName, roles) in AdministrativePolicies.RoleMap)
    {
        options.AddPolicy(policyName, policy => policy.RequireRole(roles.Select(role => role.ToString())));
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
app.UseAuthorization();
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
app.MapCatalogEndpoints();
app.MapPublicOrderingEndpoints();
app.MapAdministrationEndpoints();

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/_test/tenant", (ITenantContext tenantContext) => Results.Ok(new
    {
        tenantId = tenantContext.GetRequiredTenantId()
    }));
}

app.Run();

public partial class Program;
