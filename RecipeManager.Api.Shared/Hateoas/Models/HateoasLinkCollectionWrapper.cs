namespace RecipeManager.Api.Shared.Hateoas.Models;

public class HateoasLinkCollectionWrapper<TItem>(ICollection<HateoasResponse<TItem>> items)
{
    public ICollection<HateoasResponse<TItem>> Items { get; init; } = items;

    public ICollection<Link> Links { get; init; } = [];
}
