namespace RecipeManager.Api.Domain.Recipes;

public record RecipeCategoryId(Guid Value)
{
    public static implicit operator Guid(RecipeCategoryId id) => id.Value;

    public static implicit operator RecipeCategoryId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
