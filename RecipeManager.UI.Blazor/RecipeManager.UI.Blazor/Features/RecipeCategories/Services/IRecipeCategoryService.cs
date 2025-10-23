using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Features.RecipeCategories.Models;

namespace RecipeManager.UI.Blazor.Features.RecipeCategories.Services;

public interface IRecipeCategoryService
{
    Task<HateoasResponse<RecipeCategoryPageModel>> GetCategories(int pageNumber, int pageSie, int? categoryType);

    Task<IEnumerable<RecipeCategoryForDropdownModel>> GetCategoriesForDropdown(string? categoryName = null);

    Task<Dictionary<RecipeCategoryType, IEnumerable<RecipeCategoryForDropdownModel>>> GetCategoriesForFiltering();

    Task CreateCategory(RecipeCategoryForCreateModel category);

    Task DeleteCategory(string relativeUri);

}
