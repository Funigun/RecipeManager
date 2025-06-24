using RecipeManager.UI.Blazor.Features.Account.Login;
using RecipeManager.UI.Blazor.Features.Account.Register;

namespace RecipeManager.UI.Blazor.Brokers.IdentityApi;

public class IdentityApi(HttpClient httpClient) : ApiBroker(httpClient), IIdentityApi
{
    public async Task<HttpResponseMessage> LoginUser(LoginRequest userLoginModel)
    {
        return await Post(new Uri("api/auth/login"), userLoginModel);
    }

    public async Task<HttpResponseMessage> RegisterUser(RegistrationRequest userRegistrationModel)
    {
        return await Post(new Uri("api/account/register"), userRegistrationModel);
    }
}
