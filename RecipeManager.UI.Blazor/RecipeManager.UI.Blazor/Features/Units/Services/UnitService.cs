using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Features.Units.GetUnits;

namespace RecipeManager.UI.Blazor.Features.Units.Services;

public class UnitService(IRecipeApi recipeApi) : IUnitService
{
    public async Task<HateoasCollectionResponse<UnitModel>> GetUnits()
    {
        HttpResponseMessage response = await recipeApi.GetUnits();

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasCollectionResponse<UnitModel>>())!;
        }

        return new();
    }
}
