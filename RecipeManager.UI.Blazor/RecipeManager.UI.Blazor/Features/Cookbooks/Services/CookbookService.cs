using Microsoft.AspNetCore.Components;
using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Components.Extensions;
using RecipeManager.UI.Blazor.Features.Cookbooks.GetCookbooks;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Services;

public sealed class CookbookService(IRecipeApi recipeApi, ISnackbar snackbar, NavigationManager navigationManager) : ICookbookService
{
    private const string CookbooksPageUrl = "/recipes";
    private const string CreateCookbookPageUrl = "/recipes/create";
    private const string UpdateCookbookPageUrl = "/recipes/update";

    private const string CookbooksApiUrl = "api/recipes";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task CreateCookbook(object cookbook)
    {
        HttpResponseMessage response = await recipeApi.Create(new Uri(CookbooksApiUrl, UriKind.Relative), cookbook);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(CookbooksPageUrl);
            snackbar.ShowSuccess("Recipe added sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        }
    }

    public async Task<HateoasResponse<object>> GetCookbookForManageById(Guid cookbookId)
    {
        HttpResponseMessage response = await recipeApi.GetById(new Uri($"{CookbooksApiUrl}/{cookbookId}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<object>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        navigationManager.NavigateTo(CookbooksPageUrl);

        return new HateoasResponse<object>();
    }

    public async Task<HateoasResponse<CookbooksPageModel>> GetCookbooksPage(int page, int pageSize, string sortBy = "title", bool isAscending = true)
    {
        string requestUrl = $"{CookbooksApiUrl}?page={page}&pageSize={pageSize}&sortBy={sortBy}&isAscending={isAscending}";

        HttpResponseMessage response = await recipeApi.GetAll(new Uri(requestUrl, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<CookbooksPageModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        return new HateoasResponse<CookbooksPageModel>();
    }

    public async Task UpdateCookbook(Guid cookbookId, object cookbook)
    {
        HttpResponseMessage response = await recipeApi.Update(new Uri($"{CookbooksApiUrl}/{cookbookId}", UriKind.Relative), cookbook);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(CookbooksPageUrl);
            snackbar.ShowSuccess("Cookbook updated sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        }
    }

    public async Task DeleteCookbook(Guid cookbookId)
    {
        HttpResponseMessage response = await recipeApi.DeleteItem(new Uri($"{CookbooksApiUrl}/{cookbookId}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            snackbar.ShowSuccess("Cookbook deleted successfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        }
    }

    public void OpenCreateCookbookPage()
    {
        navigationManager.NavigateTo(CreateCookbookPageUrl);
    }

    public void OpenUpdateCookbookPage(Guid cookbookId)
    {
        navigationManager.NavigateTo($"{UpdateCookbookPageUrl}/{cookbookId}");
    }
}
