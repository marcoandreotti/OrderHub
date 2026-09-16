using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Application.Identity;
using OrderHub.Application.Onboarding;
using OrderHub.Contracts.Administration;

namespace OrderHub.Api.Administration;

internal static class OnboardingEndpoints
{
    public static IEndpointRouteBuilder MapOnboardingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/admin/establishments/{establishmentId:guid}").WithTags("Establishment configuration").RequireAuthorization(AdministrativePolicies.Administration);
        group.MapGet("/onboarding", async (Guid establishmentId, IQueryDispatcher d, CancellationToken ct) =>
            Results.Ok(await d.DispatchAsync<GetOnboardingQuery, OnboardingProgress>(new(establishmentId), ct)));
        group.MapPost("/onboarding/complete", async (Guid establishmentId, ICommandDispatcher d, CancellationToken ct) =>
        { await d.DispatchAsync(new CompleteOnboardingCommand(establishmentId), ct); return Results.NoContent(); });
        group.MapGet("/configuration", async (Guid establishmentId, IQueryDispatcher d, CancellationToken ct) =>
            Results.Ok(await d.DispatchAsync<GetConfigurationQuery, ConfigurationReadModel>(new(establishmentId), ct)));
        group.MapPut("/configuration", async (Guid establishmentId, EstablishmentUpdateRequest r, ICommandDispatcher d, CancellationToken ct) =>
        { await d.DispatchAsync(new UpdateEstablishmentCommand(establishmentId, r.TradeName, r.Slug), ct); return Results.NoContent(); });
        group.MapPut("/theme", async (Guid establishmentId, EstablishmentThemeRequest r, ICommandDispatcher d, CancellationToken ct) =>
        { await d.DispatchAsync(new UpdateThemeCommand(establishmentId, new(r.PrimaryColor, r.SecondaryColor, r.BackgroundColor, r.TextColor, r.FontFamily, r.LogoUrl, r.FaviconUrl)), ct); return Results.NoContent(); });
        group.MapPut("/business-hours", async (Guid establishmentId, ReplaceBusinessHoursRequest r, ICommandDispatcher d, CancellationToken ct) =>
        { await d.DispatchAsync(new ReplaceBusinessHoursCommand(establishmentId, r.Hours?.Select(h => h is null ? null! : new HoursInput((DayOfWeek)h.DayOfWeek, h.OpensAt, h.ClosesAt)).ToArray()!), ct); return Results.NoContent(); });
        group.MapGet("/tables", async (Guid establishmentId, int? page, int? pageSize, IQueryDispatcher d, CancellationToken ct) =>
            Results.Ok(await d.DispatchAsync<SearchTablesQuery, TableSearchResult>(new(establishmentId, page ?? 1, pageSize ?? 20), ct)));
        group.MapPost("/tables", async (Guid establishmentId, CreateTableRequest r, ICommandDispatcher d, CancellationToken ct) =>
            Results.Ok(new { id = await d.DispatchAsync<CreateTableCommand, Guid>(new(establishmentId, r.IntentId, r.Code, r.Description), ct) }));
        group.MapPut("/tables/{tableId:guid}", async (Guid establishmentId, Guid tableId, UpdateTableRequest r, ICommandDispatcher d, CancellationToken ct) =>
        { await d.DispatchAsync(new UpdateTableCommand(establishmentId, tableId, r.Code, r.Description, r.IsActive), ct); return Results.NoContent(); });
        group.MapPost("/tables/{tableId:guid}/rotate-token", async (Guid establishmentId, Guid tableId, ICommandDispatcher d, CancellationToken ct) =>
        { await d.DispatchAsync(new RotateTableTokenCommand(establishmentId, tableId), ct); return Results.NoContent(); });
        return endpoints;
    }
}
