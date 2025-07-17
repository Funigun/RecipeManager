namespace RecipeManager.Api.Domain.Recipes;

public record struct RecipeCategoryId(Guid Value)
{
    public override readonly string ToString() => Value.ToString();
}
