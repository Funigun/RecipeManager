using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Persistance.Extensions;

public static class UnitRepositoryExtentions
{
    extension(IQueryable<Unit> query)
    {
        public IOrderedQueryable<Unit> DefaultOrder() => query.OrderBy(unit => unit.Group)
                                                                .ThenBy(unit => unit.PrimaryUnit == null ? 1 : 2)
                                                                .ThenBy(unit => unit.PrimaryUnit != null ? unit.ConversionFactor : 1000)
                                                                .ThenBy(unit => unit.Name);
    }
}
