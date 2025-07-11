using RecipeManager.UI.Blazor.Features.Units.CreateUnit;
using RecipeManager.UI.Blazor.Features.Units.UpdateUnit;

namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public interface IRecipeApi
{
    Task<HttpResponseMessage> CreateUnit(UnitForCreateModel unit);

    Task<HttpResponseMessage> GetUnitById(Guid unitId);

    Task<HttpResponseMessage> GetUnits();

    Task<HttpResponseMessage> UpdateUnit(Guid unitId, UnitForUpdateModel unit);

    Task<HttpResponseMessage> DeleteUnit(Guid unitId);
}
