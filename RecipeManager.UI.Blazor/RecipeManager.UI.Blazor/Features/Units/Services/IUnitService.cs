using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Units.CreateUnit;
using RecipeManager.UI.Blazor.Features.Units.GetUnits;

namespace RecipeManager.UI.Blazor.Features.Units.Services;

public interface IUnitService
{
    ApiResponseBody ResponseBody { get; }

    Task<HateoasCollectionResponse<UnitModel>> GetUnits();

    void OpenCreateUnitPage();

    Task CreateUnit(UnitForCreateModel unit);
}
