using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public class HateoasCollectionListBuilder<TItem> : HateoasCollectionResponseBuilder<TItem>
{
    public HateoasCollectionListBuilder(IEnumerable<HateoasResponse<TItem>> items, List<Link> links) : base(items, links)
    {
    }

    public HateoasCollectionListBuilder<TItem> WithGet(Func<TItem, string> href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            foreach (var item in _items)
            {
                item.Links.Add(Link.CreateGet(href(item.Item), rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithPost(Func<TItem, string> href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            foreach (var item in _items)
            {
                item.Links.Add(Link.CreatePost(href(item.Item), rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithPut(Func<TItem, string> href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            foreach (var item in _items)
            {
                item.Links.Add(Link.CreatePut(href(item.Item), rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithDelete(Func<TItem, string> href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            foreach (var item in _items)
            {
                item.Links.Add(Link.CreateDelete(href(item.Item), rel));
            }
        }

        return this;
    }
}
