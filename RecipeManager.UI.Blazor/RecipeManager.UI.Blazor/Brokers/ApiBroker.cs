namespace RecipeManager.UI.Blazor.Brokers;

public abstract class ApiBroker(HttpClient httpClient)
{
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
