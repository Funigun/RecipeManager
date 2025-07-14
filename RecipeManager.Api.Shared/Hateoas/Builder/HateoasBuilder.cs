using RecipeManager.Api.Shared.Hateoas.Common;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public class HateoasBuilder<TType>(HateoasLinkService linkService)
{
    public HateoasLinkService LinkService { get; protected set; } = linkService;

    public ICollection<IHateoasResponseBuilder> Builders { get; protected set; } = [];

    private IHateoasResponseBuilder _currentBuilder = default!;

    public HateoasResponseBuilder<TItem> ForItem<TItem>(TItem dto)
    {
        HateoasResponseBuilder<TItem> builder = new(dto, LinkService);
        _currentBuilder = builder;
        Builders.Add(builder);

        return builder;
    }

    public HateoasCollectionResponseBuilder<TItem> ForCollection<TItem>(IEnumerable<TItem> collection)
    {
        HateoasCollectionResponseBuilder<TItem> builder = new(collection, LinkService);
        _currentBuilder = builder;
        Builders.Add(builder);

        return builder;
    }

    public TType BuildResults()
    {
        return (TType)_currentBuilder.Build();
    }

    public object BuildResponses()
    {
        if (Builders.Count == 1)
        {
            return Builders.First().Build();
        }

        List<object> results = [];

        foreach (IHateoasResponseBuilder builder in Builders)
        {
            results.Add(builder.Build());
        }

        return results;
    }
}
