using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public sealed class RecipeApi(HttpClient httpClient, ProtectedLocalStorage localStorage) : ApiBroker(httpClient, localStorage), IRecipeApi
{
    public async Task<HttpResponseMessage> Create<TItem>(Uri uri, TItem item)
    {
        await AddAuthorizationHeader();
        return await Post(uri, item);
    }

    public async Task<HttpResponseMessage> GetById(Uri uri)
    {
        await AddAuthorizationHeader();
        return await Get(uri);
    }

    public async Task<HttpResponseMessage> GetAll(Uri uri)
    {
        await AddAuthorizationHeader();
        return await Get(uri);
    }

    public async Task<HttpResponseMessage> Update<TItem>(Uri uri, TItem item)
    {
        await AddAuthorizationHeader();
        return await Put(uri, item);
    }

    public async Task<HttpResponseMessage> DeleteItem(Uri uri)
    {
        await AddAuthorizationHeader();
        return await Delete(uri);
    }
}
