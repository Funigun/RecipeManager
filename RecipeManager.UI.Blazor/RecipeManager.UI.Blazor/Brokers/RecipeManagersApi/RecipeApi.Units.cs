using RecipeManager.UI.Blazor.Features.Units.CreateUnit;

namespace RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;

public sealed partial class RecipeApi
{
    public async Task<HttpResponseMessage> CreateUnit(UnitForCreateModel unit)
    {
        await AddAuthorizationHeader();

        return await Post(new Uri("api/units", UriKind.Relative), unit);
    }

    public async Task<HttpResponseMessage> GetUnits()
    {
        await AddAuthorizationHeader();
        return await Get(new Uri("api/units", UriKind.Relative));
    }

    public async Task<HttpResponseMessage> UpdateUnit(Guid unitId)
    {
        await AddAuthorizationHeader();
        return await Put(new Uri($"api/units/{unitId}", UriKind.Relative), "");
    }

    public async Task<HttpResponseMessage> DeleteUnit(Guid unitId)
    {
        await AddAuthorizationHeader();
        return await Delete(new Uri($"api/units/{unitId}", UriKind.Relative));
    }
}
