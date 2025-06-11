using Microsoft.AspNetCore.Routing;
using RecipeManager.Api.Shared.Hateoas.Common;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public class HateoasBuilder(HateoasLinkService linkService)
{
    protected readonly HateoasLinkService LinkService = linkService;
    protected readonly List<IHateoasResponseBuilder> Builders = [];

    public HateoasResponseBuilder<TItem> ForItem<TItem>(TItem dto)
    {
        HateoasResponseBuilder<TItem> builder = new(dto, LinkService);

        Builders.Add(builder);

        return builder;
    }

    public HateoasCollectionResponseBuilder<TItem> ForCollection<TItem>(IEnumerable<TItem> collection)
    {
        HateoasCollectionResponseBuilder<TItem> builder = new(collection, LinkService);

        Builders.Add(builder);

        return builder;
    }

    public object BuildResponses()
    {
        if (Builders.Count == 1)
        {
            return Builders[0].Build();
        }

        List<object> results = [];

        foreach (IHateoasResponseBuilder builder in Builders)
        {
            results.Add(builder.Build());
        }

        return results;
    }
}
