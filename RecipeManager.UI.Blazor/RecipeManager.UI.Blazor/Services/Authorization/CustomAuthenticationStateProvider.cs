using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RecipeManager.UI.Blazor.Components.Pages.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RecipeManager.UI.Blazor.Services.Authorization;

public class CustomAuthenticationStateProvider(ProtectedLocalStorage localStorage) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            LoginResponse? sessionModel = (await localStorage.GetAsync<LoginResponse>("sessionState")).Value;
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

    public async Task MarkUserAsAuthenticated(LoginResponse userDto)
    {
        await localStorage.SetAsync("sessionState", userDto);
        ClaimsIdentity identity = GetClaimsIdentity(userDto.Token);
        ClaimsPrincipal user = new (identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private static ClaimsIdentity GetClaimsIdentity(string token)
    {
        JwtSecurityTokenHandler handler = new ();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        IEnumerable<Claim> claims = jwtToken.Claims;

        return new ClaimsIdentity(claims, "jwt");
    }

    public async Task MarkUserAsLoggedOut()
    {
        await localStorage.DeleteAsync("sessionState");

        ClaimsIdentity identity = new ();
        ClaimsPrincipal user = new (identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
