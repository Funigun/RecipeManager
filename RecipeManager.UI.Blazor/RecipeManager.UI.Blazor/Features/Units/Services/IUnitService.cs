using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Units.GetUnits;
using RecipeManager.UI.Blazor.Features.Units.Manage;

namespace RecipeManager.UI.Blazor.Features.Units.Services;

public interface IUnitService
{
    ApiResponseBody ResponseBody { get; }

    Task CreateUnit(UnitForManageModel unit);

    Task<HateoasResponse<UnitForManageModel>> GetUnitById(Guid id);

    Task<HateoasCollectionResponse<UnitModel>> GetUnits();

    Task<IEnumerable<PrimaryUnitDto>> GetPrimaryUnits();

    Task<IEnumerable<UnitForDropdownModel>> GetUnitsForDropdown();

    Task UpdateUnit(string relativeUri, UnitForManageModel unit);

    Task DeleteUnit(string relativeUri);

    void OpenIndexPage();

    void OpenCreateUnitPage();

    void OpenUpdateUnitPage(Guid unitId);
}
