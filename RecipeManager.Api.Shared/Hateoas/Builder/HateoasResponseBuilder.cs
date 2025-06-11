using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public sealed class HateoasResponseBuilder<TItem>(TItem item, HateoasLinkService linkService) : HateoasBuilder(linkService), IHateoasResponseBuilder
{
    private readonly TItem Item = item;
    private readonly List<Link> Links = [];

    public HateoasResponseBuilder<TItem> AddGet(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(LinkService.GenerateGet(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddPost(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(LinkService.GeneratePost(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddPut(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(LinkService.GeneratePut(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddDelete(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            Links.Add(LinkService.GenerateDelete(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public object Build()
    {
        return new HateoasResponse<TItem>(Item, Links);
    }
}
