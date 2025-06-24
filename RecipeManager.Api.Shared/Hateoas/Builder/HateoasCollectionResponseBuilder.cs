using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public class HateoasCollectionResponseBuilder<TItem> : HateoasBuilder, IHateoasResponseBuilder
{
    private readonly HateoasCollectionListBuilder<TItem> _collectionListBuilder = default!;

    public ICollection<HateoasResponse<TItem>> Items { get; protected set; }

    public ICollection<Link> Links { get; protected set; } = [];

    public HateoasCollectionResponseBuilder(IEnumerable<TItem> items, HateoasLinkService linkService) : base(linkService)
    {
        Items = items.Select(item => new HateoasResponse<TItem>(item)).ToList();
        _collectionListBuilder = new(Items, Links, linkService);
    }

    protected HateoasCollectionResponseBuilder(IEnumerable<HateoasResponse<TItem>> items, ICollection<Link> links, HateoasLinkService linkService) : base(linkService)
    {
        Items = items.ToList();
        Links = links;
    }

    public HateoasCollectionResponseBuilder<TItem> AddGet(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(LinkService.GenerateGet(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddPost(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(LinkService.GeneratePost(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddPut(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(LinkService.GeneratePut(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionResponseBuilder<TItem> AddDelete(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(LinkService.GenerateDelete(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithCollectionLink()
    {
        return _collectionListBuilder;
    }

    public object Build()
    {
        return new HateoasLinkCollectionWrapper<TItem>(Items) { Links = Links };
    }
}
