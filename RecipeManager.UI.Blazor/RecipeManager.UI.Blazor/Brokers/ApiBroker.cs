using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RecipeManager.UI.Blazor.Features.Account.Login;

namespace RecipeManager.UI.Blazor.Brokers;

public abstract class ApiBroker(HttpClient httpClient, ProtectedLocalStorage localStorage)
{
    protected async Task AddAuthorizationHeader()
    {
        LoginResponse? sessionModel = (await localStorage.GetAsync<LoginResponse>("sessionState")).Value;

        httpClient.DefaultRequestHeaders.Authorization = sessionModel is not null
                                                       ? new AuthenticationHeaderValue("Bearer", sessionModel.Token)
                                                       : null;
    }

    protected async Task<HttpResponseMessage> Get(Uri requestUri)
    {
        return await httpClient.GetAsync(requestUri);
    }

    protected async Task<HttpResponseMessage> Post<T>(Uri requestUri, T content)
    {
        return await httpClient.PostAsJsonAsync(requestUri, content);
    }

    protected async Task<HttpResponseMessage> Put<T>(Uri requestUri, T content)
    {
        return await httpClient.PutAsJsonAsync(requestUri, content);
    }

    protected async Task<HttpResponseMessage> Delete(Uri requestUri)
    {
        return await httpClient.DeleteAsync(requestUri);
    }
}
