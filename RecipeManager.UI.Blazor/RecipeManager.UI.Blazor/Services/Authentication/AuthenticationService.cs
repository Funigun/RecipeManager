using Microsoft.AspNetCore.Components.Authorization;
using RecipeManager.UI.Blazor.Brokers.IdentityApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Account.Login;
using RecipeManager.UI.Blazor.Features.Account.Register;
using RecipeManager.UI.Blazor.Services.Authorization;

namespace RecipeManager.UI.Blazor.Services.Authentication;

public class AuthenticationService(IIdentityApi identityApi, AuthenticationStateProvider authenticationStateProvider) : IAuthenticationService
{
    public ApiResponseBody ResponseBody { get; private set; } = new ApiResponseBody();

    public async Task<bool> Register(RegistrationRequest userRegistrationModel)
    {
        HttpResponseMessage response = await identityApi.RegisterUser(userRegistrationModel);

        if (!response.IsSuccessStatusCode)
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Authenticate(LoginRequest loginRequest)
    {
        HttpResponseMessage response = await identityApi.LoginUser(loginRequest);

        if (response.IsSuccessStatusCode)
        {
            LoginResponse user = (await response.Content.ReadFromJsonAsync<LoginResponse>())!;

            await ((CustomAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(user);
            return true;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return false;
    }



    public async Task Logout() => await ((CustomAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
}
