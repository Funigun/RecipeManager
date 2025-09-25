using Microsoft.AspNetCore.Components;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Ingredients.CreateIngredient;
using RecipeManager.UI.Blazor.Features.Ingredients.GetIngredients;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Services;

public sealed class IngredientService(IRecipeApi recipeApi, NavigationManager navigationManager) : IIngredientService
{
    private const string IngredientsPageUrl = "/ingredients";
    private const string CreateIngredientPageUrl = "/ingredients/create";

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

    public void OpenIndexPage()
    {
        navigationManager.NavigateTo(IngredientsPageUrl);
    }

    public void OpenCreatePage()
    {
        navigationManager.NavigateTo(CreateIngredientPageUrl);
    }
}
