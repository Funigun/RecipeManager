namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public interface IRecipeApi
{
    Task<HttpResponseMessage> Create<TItem>(string relativeUri, TItem item);

    Task<HttpResponseMessage> GetById(string relativeUri);

    Task<HttpResponseMessage> GetAll(string relativeUri);

    Task<HttpResponseMessage> Update<TItem>(string relativeUri, TItem item);

    Task<HttpResponseMessage> Delete(string relativeUri);
}
