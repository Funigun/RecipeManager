using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Cookbooks.GetCookbooks;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Services;

public interface ICookbookService
{
    ApiResponseBody ResponseBody { get; }

    Task CreateCookbook(object cookbook);

    Task DeleteCookbook(Guid cookbookId);

    Task<HateoasResponse<object>> GetCookbookForManageById(Guid cookbookId);

    Task<HateoasResponse<CookbooksPageModel>> GetCookbooksPage(int page, int pageSize, string sortBy = "title", bool isAscending = true);

    void OpenCreateCookbookPage();

    void OpenUpdateCookbookPage(Guid cookbookId);

    Task UpdateCookbook(Guid cookbookId, object cookbook);
}
