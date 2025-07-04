using RecipeManager.API.Domain.Units;
using RecipeManager.API.Domain.Units.Enums;
using RecipeManager.API.Persistance;

namespace RecipeManager.Tests.IntegrationTests.CoreApi.TestFixtures;

internal static class DatabaseSeeder
{
    public static async Task SeedAsync(this AppDbContext dbContext)
    {
        dbContext.Units.AddRange
        (
            Unit.Create("Duplicated Name", "TU", UnitGroup.Weight),
            Unit.Create("Test", "Duplicated Short Name", UnitGroup.Weight)
        );

        await dbContext.SaveChangesAsync();
    }
}
