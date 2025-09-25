using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Ingredients.CreateIngredient;
using RecipeManager.UI.Blazor.Features.Ingredients.GetIngredients;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Services;

public interface IIngredientService
{
    ApiResponseBody ResponseBody { get; }

    Task<Guid> CreateIngredient(IngredientForCreateModel ingredientForCreate, CancellationToken cancellationToken = default);

    Task<HateoasResponse<IngredientsPageModel>> GetIngredientsPage(int page, int pageSize, string category = "", CancellationToken cancellationToken = default);

    void OpenIndexPage();

    void OpenCreatePage();
}
