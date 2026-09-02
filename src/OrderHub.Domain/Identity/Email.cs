using System.Net.Mail;
using OrderHub.Domain.Exceptions;

namespace OrderHub.Domain.Identity;

public sealed record Email
{
    public string Value { get; }
    public string NormalizedValue { get; }

    public Email(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length > 150 || !MailAddress.TryCreate(trimmed, out var parsed) || parsed.Address != trimmed)
        {
            throw new DomainException("A valid email with at most 150 characters is required.");
        }

        Value = trimmed;
        NormalizedValue = trimmed.ToUpperInvariant();
    }
}
