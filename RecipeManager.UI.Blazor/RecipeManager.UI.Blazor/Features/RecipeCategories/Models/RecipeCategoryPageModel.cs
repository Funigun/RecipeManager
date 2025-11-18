using System.Collections.ObjectModel;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;

namespace RecipeManager.UI.Blazor.Features.RecipeCategories.Models;

public class RecipeCategoryPageModel
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public Collection<HateoasResponse<RecipeCategoryModel>> Categories { get; set; } = [];
}
