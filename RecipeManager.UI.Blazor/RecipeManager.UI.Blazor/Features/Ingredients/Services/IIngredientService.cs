using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Ingredients.CreateIngredient;
using RecipeManager.UI.Blazor.Features.Ingredients.GetIngredients;
using RecipeManager.UI.Blazor.Features.Ingredients.UpdateIngredient;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Services;

public interface IIngredientService
{
    ApiResponseBody ResponseBody { get; }

    Task<Guid> CreateIngredient(IngredientForCreateModel ingredientForCreate, CancellationToken cancellationToken = default);

    Task<HateoasResponse<IngredientsPageModel>> GetIngredientsPage(int page, int pageSize, string category = "", CancellationToken cancellationToken = default);

    Task<HateoasResponse<IngredientForManageModel>> GetIngredientById(Guid id, CancellationToken cancellationToken = default);

    Task UpdateIngredient(string relativeUri, IngredientForUpdateModel ingredientForUpdate, CancellationToken cancellationToken = default);

    Task DeleteIngredient(string relativeUri);

    void OpenIndexPage();

    void OpenCreatePage();

    void OpenUpdateIngredientPage(Guid ingredientId);
}
