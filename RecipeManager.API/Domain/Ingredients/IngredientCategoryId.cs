namespace RecipeManager.Api.Domain.Ingredients;

public record struct IngredientCategoryId(Guid Value)
{
    public override readonly string ToString() => Value.ToString();
}
