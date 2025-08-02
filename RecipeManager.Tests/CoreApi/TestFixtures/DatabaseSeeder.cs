using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Persistance;

namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

internal static class DatabaseSeeder
{
    public static async Task SeedAsync(this AppDbContext dbContext)
    {
        dbContext.Units.AddRange
        (
            Unit.Create("Duplicated Name", "TU", UnitGroup.Weight),
            Unit.Create("Test", "Duplicated Short Name", UnitGroup.Weight),
            Unit.Create("To Delete", null, UnitGroup.Weight)
        );

        dbContext.RecipeCategories.AddRange
        (
            RecipeCategory.Create("Existing Category")
        );

        await dbContext.SaveChangesAsync();
    }
}
