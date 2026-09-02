using Microsoft.AspNetCore.Identity;
using OrderHub.Application.Abstractions.Identity;

namespace OrderHub.Infrastructure.Identity;

public sealed class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> hasher = new();
    private readonly object user = new();

    public string Hash(string password) => hasher.HashPassword(user, password);

    public bool Verify(string passwordHash, string password) =>
        hasher.VerifyHashedPassword(user, passwordHash, password) != PasswordVerificationResult.Failed;
}
