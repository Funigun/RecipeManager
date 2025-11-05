using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RecipeManager.UI.Blazor.Features.Account.Login;
using RecipeManager.UI.Blazor.Features.Account.Register;

namespace RecipeManager.UI.Blazor.Brokers.IdentityApi;

public class IdentityApi(HttpClient httpClient, ProtectedLocalStorage localStorage) : ApiBroker(httpClient, localStorage), IIdentityApi
{
    public async Task<HttpResponseMessage> LoginUser(LoginRequest userLoginModel)
    {
        return await Post(new Uri("api/auth/login", UriKind.Relative), userLoginModel);
    }

    public async Task<HttpResponseMessage> RegisterUser(RegistrationRequest userRegistrationModel)
    {
        return await Post(new Uri("api/account/register", UriKind.Relative), userRegistrationModel);
    }

    public async Task<HttpResponseMessage> RefreshUserToken(string refreshToken)
    {
        await AddAuthorizationHeader();
        return await Post(new Uri($"api/auth/refresh-token?refreshToken={Uri.EscapeDataString(refreshToken)}", UriKind.Relative), new { });
    }

    public async Task<HttpResponseMessage> LogoutUser()
    {
        await AddAuthorizationHeader();

        return await Post(new Uri("api/auth/logout", UriKind.Relative), new { });
    }
}
