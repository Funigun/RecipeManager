namespace RecipeManager.Api.Features.Units;

public static class UnitsGroup
{
    public const string Route = "/api/units";
    public const string Tag = "Measurement Units";
    public const string UnitByIdRoute = $"{Route}/{{unitId}}";
    public const string DropdownRoute = $"{Route}/dropdown";
    public const string PrimaryUnitsRoute = $"{Route}/primary-units";

    public static string UnitById(Guid unitId) => $"{Route}/{unitId}";
}
