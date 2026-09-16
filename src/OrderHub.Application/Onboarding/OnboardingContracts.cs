using OrderHub.Application.Abstractions.Commands;
using OrderHub.Application.Abstractions.Queries;
using OrderHub.Domain.Operations;
using OrderHub.Domain.Tenancy;

namespace OrderHub.Application.Onboarding;

public sealed record ThemeInput(string? PrimaryColor = null, string? SecondaryColor = null, string? BackgroundColor = null, string? TextColor = null, string? FontFamily = null, string? LogoUrl = null, string? FaviconUrl = null)
{
    public EstablishmentTheme ToDomain() => new(PrimaryColor, SecondaryColor, BackgroundColor, TextColor, FontFamily, LogoUrl, FaviconUrl);
}
public sealed record HoursInput(DayOfWeek DayOfWeek, TimeOnly OpensAt, TimeOnly ClosesAt);
public sealed record ConfigurationReadModel(string TradeName, string Slug, ThemeInput Theme, IReadOnlyList<HoursInput> Hours);
public sealed record TableReadModel(Guid Id, string Code, string? Description, bool IsActive, string? PublicPath);
public sealed record TableSearchResult(IReadOnlyList<TableReadModel> Items, long TotalCount, int Page, int PageSize);
public sealed record OnboardingProgress(bool DataReady, bool ThemeReady, bool HoursReady, bool AccessReady, int ActiveTables, DateTimeOffset? CompletedAt)
{
    public bool IsReady => DataReady && HoursReady && AccessReady;
    public string[] PendingSteps => new[] { DataReady ? null : "dados", HoursReady ? null : "horarios", AccessReady ? null : "acessos" }.OfType<string>().ToArray();
    public static OnboardingProgress Calculate(string tradeName, string slug, bool active, int validHours, int administrators, int activeTables, DateTimeOffset? completedAt) =>
        new(active && !string.IsNullOrWhiteSpace(tradeName) && !string.IsNullOrWhiteSpace(slug), true, validHours > 0, administrators > 0, activeTables, completedAt);
}
public sealed record GetOnboardingQuery(Guid EstablishmentId) : IQuery<OnboardingProgress>;
public sealed record GetConfigurationQuery(Guid EstablishmentId) : IQuery<ConfigurationReadModel>;
public sealed record SearchTablesQuery(Guid EstablishmentId, int Page = 1, int PageSize = 20) : IQuery<TableSearchResult>;
public sealed record UpdateEstablishmentCommand(Guid EstablishmentId, string TradeName, string Slug) : ICommand;
public sealed record UpdateThemeCommand(Guid EstablishmentId, ThemeInput Theme) : ICommand;
public sealed record ReplaceBusinessHoursCommand(Guid EstablishmentId, IReadOnlyList<HoursInput> Hours) : ICommand;
public sealed record CreateTableCommand(Guid EstablishmentId, Guid IntentId, string Code, string? Description) : ICommand<Guid>;
public sealed record UpdateTableCommand(Guid EstablishmentId, Guid TableId, string Code, string? Description, bool IsActive) : ICommand;
public sealed record RotateTableTokenCommand(Guid EstablishmentId, Guid TableId) : ICommand;
public sealed record CompleteOnboardingCommand(Guid EstablishmentId) : ICommand;

public interface IEstablishmentConfigurationRepository
{
    Task<Establishment> GetAsync(Guid tenantId, Guid establishmentId, CancellationToken ct);
    Task<IReadOnlyList<BusinessHours>> GetHoursAsync(Guid tenantId, Guid establishmentId, CancellationToken ct);
    void ReplaceHours(IReadOnlyList<BusinessHours> previous, IReadOnlyList<BusinessHours> replacement);
    Task<ServiceTable?> GetTableAsync(Guid tenantId, Guid establishmentId, Guid tableId, CancellationToken ct);
    Task<ServiceTable?> FindTableIntentAsync(Guid tenantId, Guid establishmentId, Guid intentId, CancellationToken ct);
    void AddTable(ServiceTable table);
    Task<int> CountAdministratorsAsync(Guid tenantId, Guid establishmentId, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
public interface IOnboardingReadGateway
{
    Task<OnboardingProgress> GetProgressAsync(Guid tenantId, Guid establishmentId, CancellationToken ct);
    Task<ConfigurationReadModel> GetConfigurationAsync(Guid tenantId, Guid establishmentId, CancellationToken ct);
    Task<TableSearchResult> SearchTablesAsync(Guid tenantId, SearchTablesQuery query, CancellationToken ct);
}
