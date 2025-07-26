namespace RecipeManager.Api.Domain.Cookbooks;

public sealed record CookbookCategoryId(Guid Value)
{
    public static implicit operator Guid(CookbookCategoryId id) => id.Value;

    public static implicit operator CookbookCategoryId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
