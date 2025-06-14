using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RecipeManager.Shared.Contracts.User.Login;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RecipeManager.UI.Blazor.Services.Authorization;

public class CustomAuthenticationStateProvider(ProtectedLocalStorage localStorage) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            UserLoginResponseDto? sessionModel = (await localStorage.GetAsync<UserLoginResponseDto>("sessionState")).Value;
            ClaimsIdentity identity = sessionModel == null ? new () : GetClaimsIdentity(sessionModel.Token);
            ClaimsPrincipal user = new (identity);

            return new AuthenticationState(user);
        }
        catch (Exception)
        {
            await MarkUserAsLoggedOut();
            ClaimsIdentity identity = new ClaimsIdentity();
            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
    }

    public async Task MarkUserAsAuthenticated(UserLoginResponseDto userDto)
    {
        await localStorage.SetAsync("sessionState", userDto);
        ClaimsIdentity identity = GetClaimsIdentity(userDto.Token);
        ClaimsPrincipal user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private ClaimsIdentity GetClaimsIdentity(string token)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        IEnumerable<Claim> claims = jwtToken.Claims;

        return new ClaimsIdentity(claims, "jwt");
    }

    public async Task MarkUserAsLoggedOut()
    {
        await localStorage.DeleteAsync("sessionState");
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
