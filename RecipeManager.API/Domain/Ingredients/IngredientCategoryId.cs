namespace RecipeManager.Api.Domain.Ingredients;

public record IngredientCategoryId(Guid Value)
{
    public static implicit operator Guid(IngredientCategoryId id) => id.Value;

    public static implicit operator IngredientCategoryId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
