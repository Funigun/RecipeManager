using Microsoft.AspNetCore.Components;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Units.CreateUnit;
using RecipeManager.UI.Blazor.Features.Units.GetUnits;
using RecipeManager.UI.Blazor.Features.Units.UpdateUnit;

namespace RecipeManager.UI.Blazor.Features.Units.Services;

public class UnitService(IRecipeApi recipeApi, NavigationManager navigationManager) : IUnitService
{
    private const string UnitsEndpoint = "/admin/measurement-units";
    private const string CreateUnitsEndpoint = "/admin/measurement-units/create";
    private const string UpdateUnitsEndpoint = "/admin/measurement-units/update";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task CreateUnit(UnitForCreateModel unit)
    {
        HttpResponseMessage response = await recipeApi.CreateUnit(unit);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(UnitsEndpoint);
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
    }

    public async Task<HateoasResponse<UnitForUpdateModel>> GetUnitById(Guid unitId)
    {
        HttpResponseMessage response = await recipeApi.GetUnitById(unitId);

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<UnitForUpdateModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return new HateoasResponse<UnitForUpdateModel>();
    }

    public async Task<HateoasCollectionResponse<UnitModel>> GetUnits()
    {
        HttpResponseMessage response = await recipeApi.GetUnits();

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasCollectionResponse<UnitModel>>())!;
        }

        return new();
    }

    public async Task UpdateUnit(Guid unitId, UnitForUpdateModel unit)
    {
        HttpResponseMessage response = await recipeApi.UpdateUnit(unitId, unit);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(UnitsEndpoint);
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        }
    }

    public async Task DeleteUnit(HateoasResponse<UnitModel> unit)
    {
        Link? deleteLink = unit.Links.FirstOrDefault(x => x.Rel == "delete");

        if (deleteLink is not null)
        {
            HttpResponseMessage response = await recipeApi.DeleteUnit(unit.Item.UnitId);

            if (response.IsSuccessStatusCode)
            {
                navigationManager.NavigateTo(UnitsEndpoint);
            }
        }
    }

    public void OpenIndexPage()
    {
        navigationManager.NavigateTo(UnitsEndpoint);
    }

    public void OpenCreateUnitPage()
    {
        navigationManager.NavigateTo(CreateUnitsEndpoint);
    }

    public void OpenUpdateUnitPage(Guid unitId)
    {
        navigationManager.NavigateTo($"{UpdateUnitsEndpoint}/{unitId}");
    }
}
