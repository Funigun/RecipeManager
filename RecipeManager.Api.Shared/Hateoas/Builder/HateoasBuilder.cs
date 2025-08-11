using RecipeManager.Api.Shared.Hateoas.Common;

namespace RecipeManager.Api.Shared.Hateoas.Builder;

public sealed class HateoasBuilder(HateoasLinkService linkService) : IHateoasBuilderFactory
{
    private HateoasLinkService LinkService { get; } = linkService;

    public HateoasResponseBuilder<TItem> ForItem<TItem>(TItem item) => new(item, LinkService);

    public HateoasCollectionResponseBuilder<TItem> ForCollection<TItem>(IEnumerable<TItem> items) => new(items, LinkService);
}
