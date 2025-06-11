using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public class HateoasCollectionListBuilder<TItem>(IEnumerable<HateoasResponse<TItem>> items, List<Link> links, HateoasLinkService linkService) : HateoasCollectionResponseBuilder<TItem>(items, links, linkService)
{
    public HateoasCollectionListBuilder<TItem> WithGet(LinkOptions options, Func<TItem, object> itemParams)
    {
        if (options.IsActionAllowed)
        {
            foreach (var item in _items)
            {
                item.Links.Add(LinkService.GenerateGet(options.Endpoint, itemParams(item.Item), options.Rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithPost(LinkOptions options, Func<TItem, object> itemParams)
    {
        if (options.IsActionAllowed)
        {
            foreach (var item in _items)
            {
                item.Links.Add(LinkService.GeneratePost(options.Endpoint, itemParams(item.Item), options.Rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithPut(LinkOptions options, Func<TItem, object> itemParams)
    {
        if (options.IsActionAllowed)
        {
            foreach (var item in _items)
            {
                item.Links.Add(LinkService.GeneratePut(options.Endpoint, itemParams(item.Item), options.Rel));
            }
        }

        return this;
    }

    public HateoasCollectionListBuilder<TItem> WithDelete(LinkOptions options, Func<TItem, object> itemParams)
    {
        if (options.IsActionAllowed)
        {
            foreach (var item in _items)
            {
                item.Links.Add(LinkService.GenerateDelete(options.Endpoint, itemParams(item.Item), options.Rel));
            }
        }

        return this;
    }
}
