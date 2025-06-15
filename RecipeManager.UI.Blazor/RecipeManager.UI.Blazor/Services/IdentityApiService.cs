using RecipeManager.Shared.Contracts.User.Login;
using RecipeManager.Shared.Contracts.User.Registration;

namespace RecipeManager.UI.Blazor.Services;

public class IdentityApiService(HttpClient httpClient)
{
    public async Task RegisterUser(UserRegistrationModel model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/register", model, cancellationToken);
    }

    public async Task SignIn(UserLoginModel userModel, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/login", userModel, cancellationToken);
    }
}
