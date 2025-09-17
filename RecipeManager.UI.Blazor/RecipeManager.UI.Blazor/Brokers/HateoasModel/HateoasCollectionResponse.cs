namespace RecipeManager.UI.Blazor.Brokers.HateoasModel;

public class HateoasCollectionResponse<TItem>
       where TItem : class, new()
{
    public ICollection<HateoasResponse<TItem>> Items { get; init; }

    public ICollection<Link> Links { get; init; } = [];

    public HateoasCollectionResponse()
    {
        Items = [];
    }

    public HateoasCollectionResponse(ICollection<HateoasResponse<TItem>> items)
    {
        Items = items;
    }
}
