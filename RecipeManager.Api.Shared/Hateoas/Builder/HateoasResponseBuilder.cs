using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public sealed class HateoasResponseBuilder<TItem>(TItem item, HateoasLinkService linkService)
{
    private readonly HateoasLinkService _linkService = linkService;

    private readonly TItem _item = item;
    private readonly List<Link> _links = [];

    public HateoasResponseBuilder<TItem> AddGet(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            _links.Add(_linkService.GenerateGet(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddPost(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            _links.Add(_linkService.GeneratePost(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddPut(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            _links.Add(_linkService.GeneratePut(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasResponseBuilder<TItem> AddDelete(LinkOptions options, object? routeValues)
    {
        if (options.IsActionAllowed)
        {
            _links.Add(_linkService.GenerateDelete(options.Endpoint, routeValues, options.Rel));
        }

        return this;
    }

    public HateoasResponse<TItem> Build()
    {
        return new HateoasResponse<TItem>(_item, _links);
    }
}
