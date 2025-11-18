using System.Collections.ObjectModel;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.GetCookbooks;

public sealed class CookbooksPageModel
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public int TotalCount { get; set; }

    public Collection<HateoasResponse<CookbookModel>> Cookbooks { get; set; } = [];
}
