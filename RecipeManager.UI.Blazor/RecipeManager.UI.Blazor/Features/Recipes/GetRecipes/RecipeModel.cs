namespace RecipeManager.UI.Blazor.Features.Recipes.GetRecipes;

public sealed class RecipeModel
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string? ImageURL { get; set; }
}
