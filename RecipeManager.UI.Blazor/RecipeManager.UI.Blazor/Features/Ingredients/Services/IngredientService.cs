using Microsoft.AspNetCore.Components;
using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Components.Extensions;
using RecipeManager.UI.Blazor.Features.Ingredients.CreateIngredient;
using RecipeManager.UI.Blazor.Features.Ingredients.GetIngredients;
using RecipeManager.UI.Blazor.Features.Ingredients.UpdateIngredient;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Services;

public sealed class IngredientService(IRecipeApi recipeApi, NavigationManager navigationManager, ISnackbar snackbar) : IIngredientService
{
    private const string IngredientsPageUrl = "/ingredients";
    private const string CreateIngredientPageUrl = "/ingredients/create";
    private const string UpdateIngredientPageUrl = "/ingredients/update";

    private const string IngredientsApiUrl = "api/ingredients";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task<Guid> CreateIngredient(IngredientForCreateModel ingredientForCreate, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await recipeApi.Create(new Uri(IngredientsApiUrl, UriKind.Relative), ingredientForCreate);

        if (response.IsSuccessStatusCode)
        {
            IngredientCreationResponse createdIngredientId = await response.Content.ReadFromJsonAsync<IngredientCreationResponse>(cancellationToken: cancellationToken);
            return createdIngredientId!.Id;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>(cancellationToken: cancellationToken))!;

        return Guid.Empty;
    }

    public async Task<HateoasResponse<IngredientsPageModel>> GetIngredientsPage(int page, int pageSize, string category = "", CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri($"{IngredientsApiUrl}?page={page}&pageSize={pageSize}&category={category}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<IngredientsPageModel>>(cancellationToken: cancellationToken))!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>(cancellationToken: cancellationToken))!;

        return new HateoasResponse<IngredientsPageModel>();
    }

    public async Task<HateoasResponse<IngredientForManageModel>> GetIngredientById(Guid id, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await recipeApi.GetById(new Uri($"{IngredientsApiUrl}/{id}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<IngredientForManageModel>>(cancellationToken: cancellationToken))!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>(cancellationToken: cancellationToken))!;
        return new();
    }

    public async Task UpdateIngredient(string relativeUri, IngredientForUpdateModel ingredientForUpdate, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await recipeApi.Update(new Uri(relativeUri), ingredientForUpdate);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(IngredientsPageUrl);
            snackbar.ShowSuccess("Ingredient updated sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>(cancellationToken: cancellationToken))!;
        }
    }

    public async Task DeleteIngredient(string relativeUri)
    {
        HttpResponseMessage response = await recipeApi.DeleteItem(new Uri(relativeUri));

        if (response.IsSuccessStatusCode)
        {
            navigationManager.Refresh(true);
            snackbar.ShowSuccess("Ingredient deleted sucessfully");
        }
    }

    public void OpenIndexPage()
    {
        navigationManager.NavigateTo(IngredientsPageUrl);
    }

    public void OpenCreatePage()
    {
        navigationManager.NavigateTo(CreateIngredientPageUrl);
    }

    public void OpenUpdateIngredientPage(Guid ingredientId)
    {
        navigationManager.NavigateTo($"{UpdateIngredientPageUrl}/{ingredientId}");
    }
}
