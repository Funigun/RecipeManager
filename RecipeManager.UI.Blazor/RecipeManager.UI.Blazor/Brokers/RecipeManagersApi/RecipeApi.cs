using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public sealed partial class RecipeApi(HttpClient httpClient, ProtectedLocalStorage localStorage) : ApiBroker(httpClient, localStorage), IRecipeApi
{

}
