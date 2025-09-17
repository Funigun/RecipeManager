namespace RecipeManager.UI.Blazor.Brokers.HateoasModel;

public sealed class HateoasResponse<TItem>
              where TItem : class, new()
{
    public TItem Item { get; init; } = new();

    public ICollection<Link> Links { get; set; } = [];

    public HateoasResponse()
    {

    }

    public HateoasResponse(TItem value)
    {
        Item = value;
    }

    public HateoasResponse(TItem value, ICollection<Link> links) : this(value)
    {
        Links = links;
    }
}
