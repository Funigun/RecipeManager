using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public class HateoasCollectionResponseBuilder<TItem> : HateoasBuilder, IHateoasResponseBuilder
{
    protected readonly List<HateoasResponse<TItem>> _items;
    protected readonly List<Link> _links = [];

    private readonly HateoasCollectionListBuilder<TItem> _collectionListBuilder = default!;

    protected HateoasCollectionResponseBuilder(IEnumerable<HateoasResponse<TItem>> items, List<Link> links, HateoasLinkService linkService) : base(linkService)
    {
        _items = items.ToList();
        _links = links;
    }

    public HateoasCollectionResponseBuilder(IEnumerable<TItem> items, HateoasLinkService linkService) : base(linkService)
    {
        _items = items.Select(item => new HateoasResponse<TItem>(item)).ToList();
        _collectionListBuilder = new(_items, _links, linkService);
    }

    public HateoasCollectionResponseBuilder<TItem> AddGet(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            _links.Add(LinkService.GenerateGet(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddPost(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            _links.Add(LinkService.GeneratePost(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddPut(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            _links.Add(LinkService.GeneratePut(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddDelete(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            _links.Add(LinkService.GenerateDelete(options.Endpoint, routeValues, options.Rel));
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
