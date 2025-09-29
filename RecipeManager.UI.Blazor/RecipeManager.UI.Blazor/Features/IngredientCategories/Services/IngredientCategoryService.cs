using Microsoft.AspNetCore.Components;
using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Components.Extensions;
using RecipeManager.UI.Blazor.Features.IngredientCategories.Models;

namespace RecipeManager.UI.Blazor.Features.IngredientCategories.Services;

public sealed class IngredientCategoryService(IRecipeApi recipeApi, ISnackbar snackbar, NavigationManager navigationManager) : IIngredientCategoryService
{
    private const string CategoriesApiUrl = "api/ingredientcategories";
    private const string CategoriesDropdownApiUrl = $"{CategoriesApiUrl}/dropdown";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task<HateoasCollectionResponse<IngredientCategoryForManageModel>> GetCategories()
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri(CategoriesApiUrl, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasCollectionResponse<IngredientCategoryForManageModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return new HateoasCollectionResponse<IngredientCategoryForManageModel>();
    }

    public async Task<IEnumerable<IngredientCategoryForDropdownModel>> GetCategoriesForDropdown(string? categoryName = null)
    {
        string url = string.IsNullOrEmpty(categoryName) ? CategoriesDropdownApiUrl : $"{CategoriesDropdownApiUrl}?categoryName={categoryName}";

        HttpResponseMessage response = await recipeApi.GetAll(new Uri(url, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IEnumerable<IngredientCategoryForDropdownModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return [];
    }

    public async Task CreateCategory(IngredientCategoryForCreateModel category)
    {
        HttpResponseMessage response = await recipeApi.Create(new Uri(CategoriesApiUrl, UriKind.Relative), category);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.Refresh(true);
            snackbar.ShowSuccess("Category created sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
            snackbar.ShowError(string.Join('\n', ResponseBody.GetAllErrors()));
        }
    }

    public async Task DeleteCategory(string relativeUri)
    {
        HttpResponseMessage response = await recipeApi.DeleteItem(new Uri(relativeUri));

        if (response.IsSuccessStatusCode)
        {
            navigationManager.Refresh(true);
            snackbar.ShowSuccess("Category deleted sucessfully");
        }
    }
}
