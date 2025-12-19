using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
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
            Unit.Create("Duplicated Name", "TU", "Duplicated Name", "TUs", UnitGroup.Weight, false, null, 1),
            Unit.Create("Test", "Duplicated Short Name", "Test", "Duplicated Short Name", UnitGroup.Weight, false, null, 1),
            Unit.Create("To Update", null, "To Update", null, UnitGroup.Weight, false, null, 1),
            Unit.Create("To Delete", null, "To Delete", null, UnitGroup.Weight, false, null, 1),
            Unit.Create("Existing Unit", "EU", "Existing Unit", "EU", UnitGroup.Weight, false, null, 1)
        ];

        IEnumerable<RecipeCategory> recipeCategories =
        [
            RecipeCategory.Create("Existing Category", RecipeCategoryType.Events),
            RecipeCategory.Create("Fake category 1", RecipeCategoryType.Events),
            RecipeCategory.Create("Fake category 2", RecipeCategoryType.Events),
            RecipeCategory.Create("To Delete", RecipeCategoryType.Events)
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

        UnitId unitId = dbContext.Units.Where(unit => unit.Name == "Existing Unit").First().Id;

        IEnumerable<Ingredient> ingredients =
        [
            Ingredient.Create("Existing Ingredient", new() { IngredientUnit = unitId }, ingredientCategories.ElementAt(1).Id, [ingredientCategories.ElementAt(0).Id], []),
            Ingredient.Create("Fake ingredient 1", new() { IngredientUnit = unitId }, ingredientCategories.ElementAt(0).Id, [ingredientCategories.ElementAt(1).Id], []),
            Ingredient.Create("Fake ingredient 2", new() { IngredientUnit = unitId }, ingredientCategories.ElementAt(0).Id, [ingredientCategories.ElementAt(2).Id], []),
            Ingredient.Create("To Delete", new() { IngredientUnit = unitId }, ingredientCategories.ElementAt(0).Id, [ingredientCategories.ElementAt(3).Id], [])
        ];

        dbContext.Ingredients.AddRange(ingredients);
        await dbContext.SaveChangesAsync();

        IEnumerable<Recipe> recipes =
        [
            Recipe.Create
            (
                "Existing recipe",
                RecipeAmount.Create(10d, units.First().Id.Value),
                2,
                RecipeDifficulty.Easy,
                new() { IngredientUnit = unitId },
                [
                    RecipeIngredient.Create(ingredients.First().Id.Value, units.First().Id.Value, 10d)
                ],
                [
                    RecipeSection.Create(1, [RecipeStep.Create(1, "Test", null)]),
                    RecipeSection.Create(2, [RecipeStep.Create(2, "Test 2", null)])
                ],
                [],
                null!
            ),
            Recipe.Create
            (
                "To Delete",
                RecipeAmount.Create(10d, units.First().Id.Value),
                2,
                RecipeDifficulty.Easy,
                new() { IngredientUnit = unitId },
                [
                    RecipeIngredient.Create(ingredients.First().Id.Value, units.First().Id.Value, 10d)
                ],
                [
                    RecipeSection.Create(1, [RecipeStep.Create(1, "Test", null)]),
                    RecipeSection.Create(2, [RecipeStep.Create(2, "Test 2", null)])
                ],
                [],
                null!
            ),
            Recipe.Create
            (
                "To Update",
                RecipeAmount.Create(10d, units.First().Id.Value),
                2,
                RecipeDifficulty.Easy,
                new() { IngredientUnit = unitId },
                [
                    RecipeIngredient.Create(ingredients.First().Id.Value, units.First().Id.Value, 10d)
                ],
                [
                    RecipeSection.Create(1, [RecipeStep.Create(1, "Test", null)]),
                    RecipeSection.Create(2, [RecipeStep.Create(1, "Test 2", null)])
                ],
                [],
                null!
            ),
        ];

        dbContext.Recipes.AddRange(recipes);
        await dbContext.SaveChangesAsync();

        IEnumerable<RecipeId> recipeIds = recipes.Where(recipe => recipe.Title != "To Delete").Select(r => r.Id);

        IEnumerable<Cookbook> cookbooks =
        [
            Cookbook.Create("Test book", "This is a test book", null, []),
            Cookbook.Create("To Delete", "This is a test book to delete", null, []),
            Cookbook.Create("To Update", "This is a test book to update", null, [])
        ];

        dbContext.Cookbooks.AddRange(cookbooks);
        await dbContext.SaveChangesAsync();

        IEnumerable<CookbookCategory> cookbookCategories =
        [
            CookbookCategory.Create
            (
                "Existing Category",
                cookbooks.First(),
                [
                      CookbookCategory.Create("Subcategory 1", cookbooks.First(), [], recipeIds.Skip(1)),
                ],
                [recipeIds.First()]
            ),
        ];

        Cookbook cookbook = cookbooks.First();
        cookbook.Update(cookbook.Title, cookbook.Description, null, cookbookCategories.ToList());
        dbContext.Cookbooks.Update(cookbook);
        await dbContext.SaveChangesAsync();
    }
}
