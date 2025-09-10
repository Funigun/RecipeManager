using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Features.RecipeCategories.Models;

namespace RecipeManager.UI.Blazor.Features.RecipeCategories.Services;

public interface IRecipeCategoryService
{
    Task<HateoasCollectionResponse<RecipeCategoryModel>> GetCategories();

    Task CreateCategory(RecipeCategoryForCreateModel category);

    Task DeleteCategory(string relativeUri);
}
