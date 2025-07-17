namespace RecipeManager.Api.Shared.Hateoas.Models;

public sealed class Link
{
    public string Href { get; init; } = default!;

    public string Rel { get; init; } = default!;

    public string Method { get; init; } = default!;

    public Link()
    {

    }

    private Link(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }

    public static Link CreateGet(string href, string rel) =>
        new(href, rel, Hateoas.Common.HateoasMethodConstants.HttpGet);

    public static Link CreatePost(string href, string rel) =>
        new(href, rel, Hateoas.Common.HateoasMethodConstants.HttpPost);

    public static Link CreatePut(string href, string rel) =>
        new(href, rel, Hateoas.Common.HateoasMethodConstants.HttpPut);

    public static Link CreateDelete(string href, string rel) =>
        new(href, rel, Hateoas.Common.HateoasMethodConstants.HttpDelete);
}
