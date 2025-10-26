using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Features.Recipes.GetRecipes;
using RecipeManager.UI.Blazor.Features.Recipes.Models;

namespace RecipeManager.UI.Blazor.Features.Recipes.Services;

public interface IRecipeService
{
    Task CreateRecipe(RecipeForManageModel recipe);

    Task<HateoasResponse<RecipeForManageModel>> GetRecipeForManageById(Guid recipeId);

    Task<HateoasResponse<RecipesPageModel>> GetRecipesPage(IEnumerable<Guid> categories, IEnumerable<Guid> ingredients, int page, int pageSize);

    Task UpdateRecipe(string relativeUri, RecipeForManageModel recipe);

    Task DeleteRecipe(string relativeUri);

    void OpenCreateRecipePage();

    void OpenUpdateRecipePage(Guid recipeId);
}
