namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public interface IRecipeApi
{
    Task<HttpResponseMessage> CreateUnit();

    Task<HttpResponseMessage> GetUnits();

    Task<HttpResponseMessage> UpdateUnit(Guid unitId);

    Task<HttpResponseMessage> DeleteUnit(Guid unitId);
}
