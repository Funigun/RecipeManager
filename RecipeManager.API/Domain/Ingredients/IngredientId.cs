namespace RecipeManager.Api.Domain.Ingredients;

public record struct IngredientId(Guid Value)
{
    public override readonly string ToString() => Value.ToString();
}
