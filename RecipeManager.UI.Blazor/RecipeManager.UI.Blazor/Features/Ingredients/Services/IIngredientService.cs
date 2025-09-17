using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Features.Ingredients.GetIngredients;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Services;

public interface IIngredientService
{
    Task<HateoasResponse<IngredientsPageModel>> GetIngredientsPage(int page, int pageSize, string category = "", CancellationToken cancellationToken = default);
}
