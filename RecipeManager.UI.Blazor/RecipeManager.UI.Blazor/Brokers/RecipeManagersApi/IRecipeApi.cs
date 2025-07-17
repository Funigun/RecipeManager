namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public interface IRecipeApi
{
    Task<HttpResponseMessage> Create<TItem>(Uri uri, TItem item);

    Task<HttpResponseMessage> GetById(Uri uri);

    Task<HttpResponseMessage> GetAll(Uri uri);

    Task<HttpResponseMessage> Update<TItem>(Uri uri, TItem item);

    Task<HttpResponseMessage> DeleteItem(Uri uri);
}
