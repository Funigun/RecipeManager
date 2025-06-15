using Microsoft.AspNetCore.Components.Authorization;
using RecipeManager.Shared.Contracts.User.Registration;
using RecipeManager.UI.Blazor.Common.Models;
using RecipeManager.UI.Blazor.Components.Pages.Account;
using RecipeManager.UI.Blazor.Services.Authorization;

namespace RecipeManager.UI.Blazor.Services;

public class IdentityApiService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
{
    public async Task RegisterUser(UserRegistrationModel model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/register", model, cancellationToken);
    }

    public async Task<string> SignIn(LoginRequest userModel, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/login", userModel, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            LoginResponse? user = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
            if (user != null)
            {
                await ((CustomAuthenticationStateProvider)authStateProvider).MarkUserAsAuthenticated(user);
                return "";
            }
        }

        BaseApiResponse apiResponse = (await response.Content.ReadFromJsonAsync<BaseApiResponse>(cancellationToken))!;

        return string.Join('\n', apiResponse.Errors);
    }
}
