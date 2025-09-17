using RecipeManager.UI.Blazor.Brokers.HateoasModel;

namespace RecipeManager.UI.Blazor.Features.Ingredients.GetIngredients;

public sealed class IngredientsPageModel
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public IEnumerable<HateoasResponse<IngredientModel>> Ingredients { get; set; } = [];
}
