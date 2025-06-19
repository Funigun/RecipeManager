using RecipeManager.UI.Blazor.Features.Account.Login;
using RecipeManager.UI.Blazor.Features.Account.Register;

namespace RecipeManager.UI.Blazor.Brokers.IdentityApi;

public interface IIdentityApi
{
    Task<HttpResponseMessage> RegisterUser(RegistrationRequest userRegistrationModel);

    Task<HttpResponseMessage> LoginUser(LoginRequest userLoginModel);
}
