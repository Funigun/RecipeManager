using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Persistance;

namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

internal static class DatabaseSeeder
{
    public static async Task SeedAsync(this AppDbContext dbContext)
    {
        IEnumerable<Unit> units =
        [
            Unit.Create("Duplicated Name", "TU", UnitGroup.Weight),
            Unit.Create("Test", "Duplicated Short Name", UnitGroup.Weight),
            Unit.Create("To Update", null, UnitGroup.Weight),
            Unit.Create("To Delete", null, UnitGroup.Weight)
        ];

        IEnumerable<RecipeCategory> recipeCategories =
        [
            RecipeCategory.Create("Existing Category"),
            RecipeCategory.Create("Fake category 1"),
            RecipeCategory.Create("Fake category 2"),
            RecipeCategory.Create("To Delete")
        ];

        IEnumerable<IngredientCategory> ingredientCategories =
        [
            IngredientCategory.Create("Existing Category"),
            IngredientCategory.Create("Fake category 1"),
            IngredientCategory.Create("Fake category 2"),
            IngredientCategory.Create("To Delete")
        ];

        dbContext.Units.AddRange(units);
        dbContext.RecipeCategories.AddRange(recipeCategories);
        dbContext.IngredientCategories.AddRange(ingredientCategories);

        await dbContext.SaveChangesAsync();

        IEnumerable<Ingredient> ingredients =
        [
            Ingredient.Create("Existing Ingredient", [ingredientCategories.ElementAt(0).Id], []),
            Ingredient.Create("Fake ingredient 1", [ingredientCategories.ElementAt(1).Id], []),
            Ingredient.Create("Fake ingredient 2", [ingredientCategories.ElementAt(2).Id], []),
            Ingredient.Create("To Delete", [ingredientCategories.ElementAt(3).Id], [])
        ];

        dbContext.Ingredients.AddRange(ingredients);
        await dbContext.SaveChangesAsync();
    }
}
