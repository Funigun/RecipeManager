namespace RecipeManager.Api.Domain.Cookbooks;

public record CookbookId(Guid Value)
{
    public static implicit operator Guid(CookbookId id) => id.Value;

    public static implicit operator CookbookId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
