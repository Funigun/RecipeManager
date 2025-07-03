using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Tests.IntegrationTests.Common.Users;

internal static class TokenMockFactory
{
    public static string Issuer { get; } = Guid.NewGuid().ToString();

    public static string Audience { get; } = Guid.NewGuid().ToString();

    public static SecurityKey SecurityKey { get; }

    public static SigningCredentials SigningCredentials { get; }

    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
    private static readonly byte[] Key = new byte[32];

    static TokenMockFactory()
    {
        Rng.GetBytes(Key);
        SecurityKey = new SymmetricSecurityKey(Key) { KeyId = Guid.NewGuid().ToString() };
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public static string GenerateJwtToken(ICurrentUser mockedUser)
    {
        List<Claim> claims = mockedUser.Roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }
}
