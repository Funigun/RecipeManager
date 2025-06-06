namespace RecipeManager.Api.Shared.Hateoas.Builder;

public class HateoasBuilder
{
    protected readonly List<IHateoasResponseBuilder> Builders = [];

    public HateoasResponseBuilder<TItem> ForItem<TItem>(TItem dto)
    {
        HateoasResponseBuilder<TItem> builder = new(dto);

        Builders.Add(builder);

        return builder;
    }

    public HateoasCollectionResponseBuilder<TItem> ForCollection<TItem>(IEnumerable<TItem> collection)
    {
        HateoasCollectionResponseBuilder<TItem> builder = new(collection);

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
