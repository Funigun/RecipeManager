using Microsoft.AspNetCore.Components.Authorization;
using RecipeManager.UI.Blazor.Common.Models;
using RecipeManager.UI.Blazor.Features.Account.Login;
using RecipeManager.UI.Blazor.Features.Account.Register;
using RecipeManager.UI.Blazor.Services.Authorization;

namespace RecipeManager.UI.Blazor.Services;

public class IdentityApiService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
{
    public async Task<string> RegisterUser(UserRegistrationModel model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/register", model, cancellationToken);
       
        return response.IsSuccessStatusCode
               ? string.Empty
               : string.Join('\n', (await response.Content.ReadFromJsonAsync<BaseApiResponse>(cancellationToken))!.Errors);
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
