using RecipeManager.UI.Blazor.Features.Recipes.Models;

namespace RecipeManager.UI.Blazor.Features.Recipes.Services;

public interface IRecipeService
{
    Task CreateRecipe(RecipeModel recipe);

    void OpenCreateRecipePage();
}
