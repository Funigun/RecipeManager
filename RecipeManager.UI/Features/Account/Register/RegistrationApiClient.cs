using System.Net.Http.Json;

namespace RecipeManager.UI.Features.Account.Register;

public sealed class RegistrationApiClient(HttpClient httpClient)
{
    public async Task<bool> RegisterAsync(RegistrationRequest request)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/account/register", request);
        return response.IsSuccessStatusCode;
    }
}

public sealed record RegistrationRequest(string UserName, string Password, string Email);
