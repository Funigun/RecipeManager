using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Components.Extensions;
using RecipeManager.UI.Blazor.Features.Recipes.GetRecipes;
using RecipeManager.UI.Blazor.Features.Recipes.Models;

namespace RecipeManager.UI.Blazor.Features.Recipes.Services;

public sealed class RecipeService(IRecipeApi recipeApi, ISnackbar snackbar, IJSRuntime jSRuntime, NavigationManager navigationManager) : IRecipeService
{
    private const string RecipesPageUrl = "/recipes";
    private const string CreateRecipePageUrl = "/recipes/create";
    private const string UpdateRecipePageUrl = "/recipes/update";

    private const string RecipesApiUrl = "api/recipes";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task CreateRecipe(RecipeForManageModel recipe)
    {
        HttpResponseMessage response = await recipeApi.Create(new Uri(RecipesApiUrl, UriKind.Relative), recipe);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(RecipesPageUrl);
            snackbar.ShowSuccess("Recipe added sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
            throw new Exception(JsonSerializer.Serialize(ResponseBody));
        }
    }

    public async Task<HateoasResponse<RecipeForManageModel>> GetRecipeForManageById(Guid recipeId)
    {
        HttpResponseMessage response = await recipeApi.GetById(new Uri($"{RecipesApiUrl}/{recipeId}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<RecipeForManageModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        navigationManager.NavigateTo(RecipesPageUrl);

        return new HateoasResponse<RecipeForManageModel>();
    }

    public async Task<HateoasResponse<RecipesPageModel>> GetRecipesPage(IEnumerable<Guid> categories, IEnumerable<Guid> ingredients, int page, int pageSize)
    {
        string requestUrl = $"{RecipesApiUrl}?page={page}&pageSize={pageSize}";

        foreach (Guid categoryId in categories)
        {
            requestUrl += $"&categories={categoryId}";
        }

        foreach (Guid ingredientId in ingredients)
        {
            requestUrl += $"&ingredients={ingredientId}";
        }

        HttpResponseMessage response = await recipeApi.GetAll(new Uri(requestUrl, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<RecipesPageModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return new HateoasResponse<RecipesPageModel>();
    }

    public async Task<IEnumerable<RecipeForDropdownModel>> GetRecipesForDropdown(string recipeName, int numberOfRecipesToLoad)
    {
        string requestUrl = $"{RecipesApiUrl}/dropdown?recipeName={recipeName}&numberOfRecipesToLoad={numberOfRecipesToLoad}";

        HttpResponseMessage response = await recipeApi.GetAll(new Uri(requestUrl, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IEnumerable<RecipeForDropdownModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        return [];
    }

    public async Task<IEnumerable<RecipeForMealPlanModel>> GetRecipesForMealPlan(string recipeName, int numberOfRecipesToLoad)
    {
        string requestUrl = $"{RecipesApiUrl}/mealPlan?recipeName={recipeName}&numberOfRecipesToLoad={numberOfRecipesToLoad}";

        HttpResponseMessage response = await recipeApi.GetAll(new Uri(requestUrl, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IEnumerable<RecipeForMealPlanModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        return [];
    }

    public async Task UpdateRecipe(string relativeUri, RecipeForManageModel recipe)
    {
        HttpResponseMessage response = await recipeApi.Update(new Uri(relativeUri), recipe);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(RecipesPageUrl);
            snackbar.ShowSuccess("Recipe updated sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
            throw new Exception(JsonSerializer.Serialize(ResponseBody));
        }
    }

    public async Task DeleteRecipe(string relativeUri)
    {
        HttpResponseMessage response = await recipeApi.DeleteItem(new Uri(relativeUri));

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(RecipesPageUrl);
            snackbar.ShowSuccess("Recipe deleted sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
            throw new Exception(JsonSerializer.Serialize(ResponseBody));
        }
    }

    public void OpenCreateRecipePage()
    {
        navigationManager.NavigateTo(CreateRecipePageUrl);
    }

    public async Task OpenUpdateRecipePage(Guid recipeId, bool openInNewTab = false)
    {
        string url = $"{UpdateRecipePageUrl}/{recipeId}";

        if (openInNewTab)
        {
            await jSRuntime.InvokeVoidAsync("open", url, "_blank");
        }
        else
        {
            navigationManager.NavigateTo(url);
        }
    }
}
