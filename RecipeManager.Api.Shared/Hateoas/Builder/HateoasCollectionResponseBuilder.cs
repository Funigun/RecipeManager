using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public class HateoasCollectionResponseBuilder<TItem> : HateoasBuilder, IHateoasResponseBuilder
{
    protected readonly List<HateoasResponse<TItem>> _items;
    protected readonly List<Link> _links = [];

    private readonly HateoasCollectionListBuilder<TItem> _collectionListBuilder = default!;

    protected HateoasCollectionResponseBuilder(IEnumerable<HateoasResponse<TItem>> items, List<Link> links)
    {
        _items = items.ToList();
        _links = links;
    }

    public HateoasCollectionResponseBuilder(IEnumerable<TItem> items)
    {
        _items = items.Select(item => new HateoasResponse<TItem>(item)).ToList();
        _collectionListBuilder = new(_items, _links);
    }

    public HateoasCollectionResponseBuilder<TItem> AddGet(string href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            _links.Add(Link.CreateGet(href, rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddPost(string href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            _links.Add(Link.CreatePost(href, rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddPut(string href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            _links.Add(Link.CreatePut(href, rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddDelete(string href, string rel, bool isActionAllowed)
    {
        if (isActionAllowed)
        {
            _links.Add(Link.CreateDelete(href, rel));
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithCollectionLink()
    {
        return _collectionListBuilder;
    }

    public object Build()
    {
        return new HateoasLinkCollectionWrapper<TItem>(_items) { Links = _links };
    }
}
