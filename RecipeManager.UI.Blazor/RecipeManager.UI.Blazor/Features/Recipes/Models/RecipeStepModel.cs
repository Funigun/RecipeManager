namespace RecipeManager.UI.Blazor.Features.Recipes.Models;

public sealed class RecipeStepModel
{
    public int Order { get; set; }

    public string Description { get; set; }

    public string? ImageUrl { get; set; }
}
