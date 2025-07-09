using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Features.Units.GetUnits;

namespace RecipeManager.UI.Blazor.Features.Units.Services;

public interface IUnitService
{
    Task<HateoasCollectionResponse<UnitModel>> GetUnits();
}
