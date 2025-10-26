using Microsoft.AspNetCore.Components;
using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Components.Extensions;
using RecipeManager.UI.Blazor.Features.Units.CreateUnit;
using RecipeManager.UI.Blazor.Features.Units.GetUnits;
using RecipeManager.UI.Blazor.Features.Units.UpdateUnit;

namespace RecipeManager.UI.Blazor.Features.Units.Services;

public class UnitService(IRecipeApi recipeApi, ISnackbar snackbar, NavigationManager navigationManager) : IUnitService
{
    private const string UnitsPageUrl = "/admin/measurement-units";
    private const string CreateUnitPageUrl = "/admin/measurement-units/create";
    private const string UpdateUnitPageUrl = "/admin/measurement-units/update";

    private const string UnitsApiUrl = "api/units";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task CreateUnit(UnitForCreateModel unit)
    {
        HttpResponseMessage response = await recipeApi.Create(new Uri(UnitsApiUrl, UriKind.Relative), unit);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(UnitsPageUrl);
            snackbar.ShowSuccess("Unit added sucessfully");
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
    }

    public async Task<HateoasResponse<UnitForUpdateModel>> GetUnitById(Guid unitId)
    {
        HttpResponseMessage response = await recipeApi.GetById(new Uri($"{UnitsApiUrl}/{unitId}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<UnitForUpdateModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return new HateoasResponse<UnitForUpdateModel>();
    }

    public async Task<HateoasCollectionResponse<UnitModel>> GetUnits()
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri(UnitsApiUrl, UriKind.Relative));

        return response.IsSuccessStatusCode
             ? (await response.Content.ReadFromJsonAsync<HateoasCollectionResponse<UnitModel>>())!
             : new HateoasCollectionResponse<UnitModel>();
    }

    public async Task<IEnumerable<PrimaryUnitDto>> GetPrimaryUnits()
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri($"{UnitsApiUrl}/primary-units", UriKind.Relative));

        return response.IsSuccessStatusCode
             ? (await response.Content.ReadFromJsonAsync<IEnumerable<PrimaryUnitDto>>())!
             : [];
    }

    public async Task<IEnumerable<UnitForDropdownModel>> GetUnitsForDropdown()
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri($"{UnitsApiUrl}/dropdown", UriKind.Relative));

        return response.IsSuccessStatusCode
             ? (await response.Content.ReadFromJsonAsync<IEnumerable<UnitForDropdownModel>>())!
             : [];
    }

    public async Task UpdateUnit(string relativeUri, UnitForUpdateModel unit)
    {
        HttpResponseMessage response = await recipeApi.Update(new Uri(relativeUri), unit);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo(UnitsPageUrl);
            snackbar.ShowSuccess("Unit updated sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        }
    }

    public async Task DeleteUnit(string relativeUri)
    {
        HttpResponseMessage response = await recipeApi.DeleteItem(new Uri(relativeUri));

        if (response.IsSuccessStatusCode)
        {
            navigationManager.Refresh(true);
            snackbar.ShowSuccess("Unit deleted sucessfully");
        }
    }

    public void OpenIndexPage()
    {
        navigationManager.NavigateTo(UnitsPageUrl);
    }

    public void OpenCreateUnitPage()
    {
        navigationManager.NavigateTo(CreateUnitPageUrl);
    }

    public void OpenUpdateUnitPage(Guid unitId)
    {
        navigationManager.NavigateTo($"{UpdateUnitPageUrl}/{unitId}");
    }
}
