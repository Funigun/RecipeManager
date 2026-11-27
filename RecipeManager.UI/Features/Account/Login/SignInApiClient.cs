using System.Net.Http.Json;

namespace RecipeManager.UI.Features.Account.Login;

public sealed class SignInApiClient(HttpClient httpClient)
{
    public async Task<SignInResponse?> SignInAsync(SignInRequest request)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<SignInResponse>();
    }
}

public sealed record SignInRequest(string UserName, string Password);

public sealed record SignInResponse(string Token, string RefreshToken, DateTime ExpirationDate);
