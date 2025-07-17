namespace RecipeManager.Api.Domain.Recipes;

public record struct RecipeId(Guid Value)
{
    public override readonly string ToString() => Value.ToString();
}
