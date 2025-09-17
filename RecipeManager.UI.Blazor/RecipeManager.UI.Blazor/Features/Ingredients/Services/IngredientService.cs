using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Ingredients.GetIngredients;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Services;

public sealed class IngredientService(IRecipeApi recipeApi) : IIngredientService
{
    private const string IngredientsApiUrl = "api/ingredients";

    public ApiResponseBody ResponseBody { get; private set; } = new();

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
}
