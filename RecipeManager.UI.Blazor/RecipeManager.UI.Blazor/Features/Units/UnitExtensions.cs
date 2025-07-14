namespace RecipeManager.UI.Blazor.Features.Units;

public static class UnitExtensions
{
    public static string ToFriendlyString(this UnitGroup group)
    {
        return group switch
        {
            UnitGroup.Weight => "Weight",
            UnitGroup.Volume => "Volume",
            UnitGroup.Quantity => "Quantity",
            _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
        };
    }

    public static Dictionary<UnitGroup, string> GetUnitGroups()
    {
        return Enum.GetValues<UnitGroup>().ToDictionary(group => group, group => group.ToFriendlyString());
    }
}
