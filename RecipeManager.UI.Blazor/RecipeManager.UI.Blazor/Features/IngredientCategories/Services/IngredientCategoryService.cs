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

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task<HateoasCollectionResponse<IngredientCategoryModel>> GetCategories()
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri(CategoriesApiUrl, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasCollectionResponse<IngredientCategoryModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return new HateoasCollectionResponse<IngredientCategoryModel>();
    }

    public async Task CreateCategory(IngredientCategoryForCreateModel category)
    {
        HttpResponseMessage response = await recipeApi.Create(new Uri(CategoriesApiUrl, UriKind.Relative), category);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.Refresh(true);
            snackbar.ShowSuccess("Category created sucessfully");
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
