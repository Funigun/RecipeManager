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

    public HateoasResponseBuilder<TItem> AddPagedNavigation(string endpoint, object? baseRouteValues = null)
    {
        if (_item is not PagedResult paged)
        {
            throw new InvalidOperationException("Paged navigation links can only be added to items that implement PagedResult.");
        }

        if (paged.Page > 1)
        {
            AddGet(LinkOptions.Create(endpoint, HateoasRelConstants.FirstPage, true), new { Page = 1, PageSize = paged.PageSize });
            AddGet(LinkOptions.Create(endpoint, HateoasRelConstants.PreviousPage, true), new { Page = paged.Page - 1, PageSize = paged.PageSize });
        }

        if (paged.Page < paged.TotalPages)
        {
            AddGet(LinkOptions.Create(endpoint, HateoasRelConstants.NextPage, true), new { Page = paged.Page + 1, PageSize = paged.PageSize });
            AddGet(LinkOptions.Create(endpoint, HateoasRelConstants.LastPage, true), new { Page = paged.TotalPages, PageSize = paged.PageSize });
        }

        return this;
    }

    public HateoasResponse<TItem> Build()
    {
        return new HateoasResponse<TItem>(_item, _links);
    }
}
