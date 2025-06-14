using RecipeManager.Shared.Contracts.User.Login;
using RecipeManager.Shared.Contracts.User.Registration;

namespace RecipeManager.UI.Blazor.Services;

public class IdentityApiService(HttpClient httpClient, IConfiguration configuration)
{
    public async Task RegisterUser(UserRegistrationModel model, CancellationToken cancellationToken = default)
    {
        UserRegistrationDto userDto = new()
        {
            UserName = model.UserName,
            Password = model.Password,
            Email = model.Email
        };

        Wrapper<UserRegistrationDto> wrap = new() { UserDto = userDto };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/register", wrap, cancellationToken);
    }

    public async Task SignIn(UserLoginModel userModel, CancellationToken cancellationToken = default)
    {
        UserLoginDto userDto = new() { UserName = userModel.UserName, Password = userModel.Password };
        Wrapper<UserLoginDto> wrap = new () { UserDto = userDto };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/login", wrap, cancellationToken);
    }

    private class Wrapper<T> where T : class
    {
        public T UserDto { get; set; }
    }
}
