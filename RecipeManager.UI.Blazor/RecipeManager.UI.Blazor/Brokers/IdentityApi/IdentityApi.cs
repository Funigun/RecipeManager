using RecipeManager.UI.Blazor.Features.Account.Login;
using RecipeManager.UI.Blazor.Features.Account.Register;

namespace RecipeManager.UI.Blazor.Brokers.IdentityApi;

public class IdentityApi(HttpClient httpClient) : ApiBroker(httpClient), IIdentityApi
{
    public async Task<HttpResponseMessage> LoginUser(LoginRequest userLoginModel)
    {
        return await Post("api/auth/login", userLoginModel);
    }

    public async Task<HttpResponseMessage> RegisterUser(RegistrationRequest userRegistrationModel)
    {
        return await Post("api/account/register", userRegistrationModel);
    }
}
