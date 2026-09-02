using System.Text.RegularExpressions;
using OrderHub.Domain.Exceptions;

namespace OrderHub.Domain.Tenancy;

public sealed partial record Slug
{
    public string Value { get; }

    public Slug(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Replace(' ', '-');
        if (normalized.Length is < 3 or > 100 || !ValidSlug().IsMatch(normalized))
        {
            throw new DomainException("Slug must contain 3 to 100 lowercase letters, numbers, or hyphens.");
        }

        Value = normalized;
    }

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidSlug();
}
