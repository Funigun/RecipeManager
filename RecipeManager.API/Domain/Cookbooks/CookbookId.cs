namespace RecipeManager.Api.Domain.Cookbooks;

public record struct CookbookId(Guid Value)
{
    public override readonly string ToString() => Value.ToString();
}
