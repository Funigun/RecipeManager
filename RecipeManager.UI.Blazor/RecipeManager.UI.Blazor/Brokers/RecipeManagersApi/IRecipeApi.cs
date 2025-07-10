using RecipeManager.UI.Blazor.Features.Units.CreateUnit;

namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public interface IRecipeApi
{
    Task<HttpResponseMessage> CreateUnit(UnitForCreateModel unit);

    Task<HttpResponseMessage> GetUnits();

    Task<HttpResponseMessage> UpdateUnit(Guid unitId);

    Task<HttpResponseMessage> DeleteUnit(Guid unitId);
}
