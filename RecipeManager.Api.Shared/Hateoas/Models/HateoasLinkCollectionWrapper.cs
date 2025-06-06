namespace RecipeManager.Api.Shared.Hateoas.Models;

public class HateoasLinkCollectionWrapper<TItem>(List<HateoasResponse<TItem>> items)
{
    public List<HateoasResponse<TItem>> Items { get; set; } = items;

    public List<Link> Links { get; set; } = [];
}
