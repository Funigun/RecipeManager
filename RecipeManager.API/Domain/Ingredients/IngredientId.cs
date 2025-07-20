namespace RecipeManager.Api.Domain.Ingredients;

public record IngredientId(Guid Value)
{
    public static implicit operator Guid(IngredientId id) => id.Value;

    public static implicit operator IngredientId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
