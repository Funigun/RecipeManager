using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Features.IngredientCategories.Models;

namespace RecipeManager.UI.Blazor.Features.IngredientCategories.Services;

public interface IIngredientCategoryService
{
    Task<HateoasCollectionResponse<IngredientCategoryModel>> GetCategories();

    Task CreateCategory(IngredientCategoryForCreateModel category);

    Task DeleteCategory(string relativeUri);
}
