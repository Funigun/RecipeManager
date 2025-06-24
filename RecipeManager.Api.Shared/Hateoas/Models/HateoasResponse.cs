namespace RecipeManager.Api.Shared.Hateoas.Models;

public sealed class HateoasResponse<TItem>(TItem value)
{
    public TItem Item { get; init; } = value;

    public ICollection<Link> Links { get; set; } = [];

    public HateoasResponse(TItem value, ICollection<Link> links) : this(value)
    {
        Links = links;
    }
}
