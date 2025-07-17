namespace RecipeManager.UI.Blazor.Brokers.HateoasModel;

public sealed class Link
{
    public string Href { get; init; } = default!;

    public string Rel { get; init; } = default!;

    public string Method { get; init; } = default!;

    public Link()
    {

    }

    public Link(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}
