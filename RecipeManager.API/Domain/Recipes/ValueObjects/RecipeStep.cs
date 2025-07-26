namespace RecipeManager.Api.Domain.Recipes.ValueObjects;

public sealed record RecipeStep
{
    public int Order { get; set; }

    public string Description { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;

    private RecipeStep()
    {
    }

    public static RecipeStep Create(int order, string description, string? imageUrl)
    {
        return new()
        {
            Order = order,
            Description = description,
            ImageUrl = imageUrl ?? string.Empty
        };
    }
}
