using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace RecipeManager.UI.Features.Account.Shared;

public sealed class UiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public string? AccessToken { get; private set; }

    public string? RefreshToken { get; private set; }

    public DateTime? ExpirationDate { get; private set; }

    public bool IsAdministrator => _currentUser.IsInRole("Admin");

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public bool TrySignIn(string accessToken, string refreshToken, DateTime expirationDate)
    {
        if (!_tokenHandler.CanReadToken(accessToken))
        {
            return false;
        }

        JwtSecurityToken token;
        try
        {
            token = _tokenHandler.ReadJwtToken(accessToken);
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        IEnumerable<Claim> claims = token.Claims.Select(claim =>
            claim.Type is "role" or "roles"
                ? new Claim(ClaimTypes.Role, claim.Value)
                : claim);
        ClaimsIdentity identity = new(claims, "Bearer", ClaimTypes.Name, ClaimTypes.Role);
        ClaimsPrincipal principal = new(identity);

        if (token.ValidTo <= DateTime.UtcNow ||
            (!principal.IsInRole("User") && !principal.IsInRole("Admin")))
        {
            return false;
        }

        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpirationDate = expirationDate;
        _currentUser = principal;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return true;
    }
}
