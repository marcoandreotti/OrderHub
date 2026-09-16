namespace OrderHub.Contracts.Administration;

public sealed record EstablishmentUpdateRequest(string TradeName, string Slug);
public sealed record EstablishmentThemeRequest(string? PrimaryColor = null, string? SecondaryColor = null, string? BackgroundColor = null, string? TextColor = null, string? FontFamily = null, string? LogoUrl = null, string? FaviconUrl = null);
public sealed record BusinessHoursRequest(int DayOfWeek, TimeOnly OpensAt, TimeOnly ClosesAt);
public sealed record ReplaceBusinessHoursRequest(IReadOnlyList<BusinessHoursRequest> Hours);
public sealed record CreateTableRequest(Guid IntentId, string Code, string? Description);
public sealed record UpdateTableRequest(string Code, string? Description, bool IsActive);
