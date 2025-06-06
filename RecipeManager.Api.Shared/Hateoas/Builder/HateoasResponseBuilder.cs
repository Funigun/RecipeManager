using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public sealed class HateoasResponseBuilder<TItem> : HateoasBuilder, IHateoasResponseBuilder
{
    private readonly TItem Item;
    private readonly List<Link> Links = [];

    public HateoasResponseBuilder(TItem item)
    {
        Item = item;
    }

    public HateoasResponseBuilder<TItem> AddGet(string href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            Links.Add(Link.CreateGet(href, rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddPost(string href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            Links.Add(Link.CreatePost(href, rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddPut(string href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            Links.Add(Link.CreatePut(href, rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddDelete(string href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            Links.Add(Link.CreateDelete(href, rel));
        }

        return this;
    }

    public object Build()
    {
        return new HateoasResponse<TItem>(Item, Links);
    }
}
