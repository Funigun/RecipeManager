using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public sealed partial class RecipeApi(HttpClient httpClient, ProtectedLocalStorage localStorage) : ApiBroker(httpClient, localStorage), IRecipeApi
{
    public async Task<HttpResponseMessage> Create<TItem>(string uri, TItem item)
    {
        await AddAuthorizationHeader();
        return await Post(new Uri(uri, UriKind.Relative), item);
    }

    public async Task<HttpResponseMessage> GetById(string uri)
    {
        await AddAuthorizationHeader();
        return await Get(new Uri(uri, UriKind.Relative));
    }

    public async Task<HttpResponseMessage> GetAll(string uri)
    {
        await AddAuthorizationHeader();
        return await Get(new Uri(uri, UriKind.Relative));
    }

    public async Task<HttpResponseMessage> Update<TItem>(string relativeUri, TItem item)
    {
        await AddAuthorizationHeader();
        return await Put(new Uri(relativeUri), item);
    }

    public async Task<HttpResponseMessage> Delete(string uri)
    {
        await AddAuthorizationHeader();
        return await Delete(new Uri(uri));
    }
}
