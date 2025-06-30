namespace RecipeManager.API.Domain.Units.Enums;

public static class UnitGroupExtensions
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

    public static bool IsUnitGroup(this int unitGroup)
    {
        return Enum.IsDefined(typeof(UnitGroup), unitGroup);
    }
}
