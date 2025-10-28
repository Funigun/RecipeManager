using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Account.Login;
using RecipeManager.UI.Blazor.Features.Account.Register;

namespace RecipeManager.UI.Blazor.Services.Authentication;

public interface IAuthenticationService
{
    ApiResponseBody ResponseBody { get; }

    Task<bool> Authenticate(LoginRequest loginRequest);

    Task Logout();

    Task<bool> Register(RegistrationRequest userRegistrationModel);

    Task RefreshSession();
}
