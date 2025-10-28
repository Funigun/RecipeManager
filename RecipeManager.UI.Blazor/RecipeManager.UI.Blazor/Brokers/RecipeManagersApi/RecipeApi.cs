using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RecipeManager.UI.Blazor.Services.Authentication;

namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public sealed class RecipeApi(HttpClient httpClient, ProtectedLocalStorage localStorage, IAuthenticationService authService) : ApiBroker(httpClient, localStorage), IRecipeApi
{
    public async Task<HttpResponseMessage> Create<TItem>(Uri uri, TItem item)
    {
        await authService.RefreshSession();
        await AddAuthorizationHeader();
        return await Post(uri, item);
    }

    public async Task<HttpResponseMessage> GetById(Uri uri)
    {
        await authService.RefreshSession();
        await AddAuthorizationHeader();
        return await Get(uri);
    }

    public async Task<HttpResponseMessage> GetAll(Uri uri)
    {
        await authService.RefreshSession();
        await AddAuthorizationHeader();
        return await Get(uri);
    }

    public async Task<HttpResponseMessage> Update<TItem>(Uri uri, TItem item)
    {
        await authService.RefreshSession();
        await AddAuthorizationHeader();
        return await Put(uri, item);
    }

    public async Task<HttpResponseMessage> DeleteItem(Uri uri)
    {
        await authService.RefreshSession();
        await AddAuthorizationHeader();
        return await Delete(uri);
    }
}
