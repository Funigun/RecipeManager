using Microsoft.AspNetCore.Components;
using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Components.Extensions;
using RecipeManager.UI.Blazor.Features.Recipes.Models;

namespace RecipeManager.UI.Blazor.Features.Recipes.Services;

public sealed class RecipeService(IRecipeApi recipeApi, ISnackbar snackbar, NavigationManager navigationManager) : IRecipeService
{
    private const string RecipesPageUrl = "/recipes";
    private const string CreateRecipePageUrl = "/recipes/create";
    private const string UpdateRecipePageUrl = "/recipes/update";

    private const string RecipesApiUrl = "api/recipes";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task CreateRecipe(RecipeModel recipe)
    {
        HttpResponseMessage response = await recipeApi.Create(new Uri(RecipesApiUrl, UriKind.Relative), recipe);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(RecipesPageUrl);
            snackbar.ShowSuccess("Unit added sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        }
    }

    public void OpenCreateRecipePage()
    {
        navigationManager.NavigateTo(CreateRecipePageUrl);
    }
}
