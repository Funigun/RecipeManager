using Microsoft.AspNetCore.Components;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Units.CreateUnit;
using RecipeManager.UI.Blazor.Features.Units.GetUnits;

namespace RecipeManager.UI.Blazor.Features.Units.Services;

public class UnitService(IRecipeApi recipeApi, NavigationManager navigationManager) : IUnitService
{
    private const string UnitsEndpoint = "/admin/measurement-units";
    private const string CreateUnitsEndpoint = "/admin/measurement-units/create";
    private const string UpdateUnitsEndpoint = "/admin/measurement-units/update";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task<HateoasCollectionResponse<UnitModel>> GetUnits()
    {
        HttpResponseMessage response = await recipeApi.GetUnits();

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasCollectionResponse<UnitModel>>())!;
        }

        return new();
    }

    public void OpenCreateUnitPage()
    {
        navigationManager.NavigateTo(CreateUnitsEndpoint);
    }

    public async Task CreateUnit(UnitForCreateModel unit)
    {
        HttpResponseMessage response = await recipeApi.CreateUnit(unit);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(UnitsEndpoint);
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
    }
}
