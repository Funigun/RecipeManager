using RecipeManager.Shared.Contracts.User.Registration;

namespace RecipeManager.UI.Blazor.Services;

public class IdentityApiService(HttpClient httpClient, IConfiguration configuration)
{
    public async Task RegisterUser(UserRegistrationModel model, CancellationToken cancellationToken = default)
    {
        string url = configuration["IdentityApiUrl:BaseUrl"] + "/Account/register";

        UserRegistrationDto userDto = new()
        {
            UserName = model.UserName,
            Password = model.Password,
            Email = model.Email
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, userDto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Registration failed with status code {response.StatusCode}");
        }
    }
}
