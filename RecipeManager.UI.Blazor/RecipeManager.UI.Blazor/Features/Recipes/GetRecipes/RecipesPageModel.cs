using RecipeManager.UI.Blazor.Brokers.HateoasModel;

namespace RecipeManager.UI.Blazor.Features.Recipes.GetRecipes;

public sealed class RecipesPageModel
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public int TotalCount { get; set; }

    public List<HateoasResponse<RecipeModel>> Recipes { get; set; } = [];
}
