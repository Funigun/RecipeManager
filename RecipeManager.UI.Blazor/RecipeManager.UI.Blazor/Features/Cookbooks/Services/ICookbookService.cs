using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Cookbooks.GetCookbooks;
using RecipeManager.UI.Blazor.Features.Cookbooks.Models;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Services;

public interface ICookbookService
{
    ApiResponseBody ResponseBody { get; }

    Task CreateCookbook(CookbookDto cookbook);

    Task DeleteCookbook(Guid cookbookId);

    Task<HateoasResponse<CookbookForManageModel>> GetCookbookForManageById(Guid cookbookId);

    Task<HateoasResponse<CookbooksPageModel>> GetCookbooksPage(int page, int pageSize, string sortBy = "title", bool isAscending = true);

    void OpenCreateCookbookPage();

    void OpenUpdateCookbookPage(Guid cookbookId);

    Task UpdateCookbook(string relativeUrl, CookbookDto cookbook);
}
