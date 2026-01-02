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
            Unit.Create("Existing Unit", "EU", "Existing Unit", "EU", UnitGroup.Weight, false, null, 1),

            Unit.Create("Gram", "g,", "Grams", "g", UnitGroup.Weight, true, null, 1),
            Unit.Create("Mililiter", "ml,", "Mililiters", "ml", UnitGroup.Volume, true, null, 1),
            Unit.Create("Can", "can,", "Cans", "cans", UnitGroup.Quantity, true, null, 1)
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
            IngredientCategory.Create("To Delete"),
            IngredientCategory.Create("Spices")
        ];

        dbContext.Units.AddRange(units);
        dbContext.RecipeCategories.AddRange(recipeCategories);
        dbContext.IngredientCategories.AddRange(ingredientCategories);

        await dbContext.SaveChangesAsync();

        IEnumerable<Unit> subUnits =
        [
            Unit.Create("Kilogram", "kg", "Kilograms", "kgs", UnitGroup.Weight, false, units.First(unit => unit.Name == "Gram").Id, 1000),
            Unit.Create("Teaspoon", "tsp", "Teaspoons", "tsps", UnitGroup.Volume, false, units.First(unit => unit.Name == "Mililiter").Id, 5),
            Unit.Create("Tablespoon", "tbsp", "Tablepoons", "tbsps", UnitGroup.Volume, false, units.First(unit => unit.Name == "Mililiter").Id, 15),
            Unit.Create("Liter", "l", "Liters", "l", UnitGroup.Volume, false, units.First(unit => unit.Name == "Mililiter").Id, 1000),
            Unit.Create("Cup", "cup", "Cups", "cups", UnitGroup.Volume, false, units.First(unit => unit.Name == "Mililiter").Id, 250)
        ];

        dbContext.Units.AddRange(subUnits);
        await dbContext.SaveChangesAsync();

        UnitId gramId = units.First(unit => unit.Name == "Gram").Id;
        UnitId kilogramId = subUnits.First(unit => unit.Name == "Kilogram").Id;
        UnitId literId = subUnits.First(unit => unit.Name == "Liter").Id;
        UnitId mililiterId = units.First(unit => unit.Name == "Mililiter").Id;
        UnitId canId = units.First(unit => unit.Name == "Can").Id;
        UnitId unitId = units.First(unit => unit.Name == "Existing Unit").Id;
        UnitId cupId = subUnits.First(unit => unit.Name == "Cup").Id;

        IEnumerable<Ingredient> ingredients =
        [
            Ingredient.Create("Existing Ingredient", new() { IngredientUnit = unitId }, unitId, new IngredientPackage { PackageUnitId = unitId, PackageSize = 100, PackageSizeUnitId = unitId }, [], ingredientCategories.ElementAt(1).Id, [ingredientCategories.ElementAt(0).Id], []),
            Ingredient.Create("Fake ingredient 1", new() { IngredientUnit = unitId }, unitId, new IngredientPackage { PackageUnitId = unitId, PackageSize = 100, PackageSizeUnitId = unitId }, [], ingredientCategories.ElementAt(0).Id, [ingredientCategories.ElementAt(1).Id], []),
            Ingredient.Create("Fake ingredient 2", new() { IngredientUnit = unitId }, unitId, new IngredientPackage { PackageUnitId = unitId, PackageSize = 100, PackageSizeUnitId = unitId }, [], ingredientCategories.ElementAt(0).Id, [ingredientCategories.ElementAt(2).Id], []),
            Ingredient.Create("To Delete", new() { IngredientUnit = unitId }, unitId, new IngredientPackage { PackageUnitId = unitId, PackageSize = 100, PackageSizeUnitId = unitId }, [], ingredientCategories.ElementAt(0).Id, [ingredientCategories.ElementAt(3).Id], []),

            Ingredient.Create("Salt", new() { IngredientUnit = gramId, IngredientAmount = 100 }, gramId, new IngredientPackage { PackageUnitId = kilogramId, PackageSize = 1, PackageSizeUnitId = kilogramId }, [new() { UnitToConvertId = mililiterId, Ratio = 5 }], ingredientCategories.ElementAt(4).Id, [ingredientCategories.ElementAt(0).Id], []),
            Ingredient.Create("Canned tomatoes", new() { IngredientUnit = gramId }, mililiterId, new IngredientPackage { PackageUnitId = canId, PackageSize = 400, PackageSizeUnitId = mililiterId }, [new() { UnitToConvertId = canId, Ratio = 400 }, new() { UnitToConvertId = gramId, Ratio = 1.0255 }], ingredientCategories.ElementAt(2).Id, [], []),
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

        // Add new ingredients for nested recipe and shopping list scenarios
        Ingredient wholeChicken = Ingredient.Create("Whole chicken", new() { IngredientUnit = kilogramId, IngredientAmount = 1 }, kilogramId, new IngredientPackage { PackageUnitId = kilogramId, PackageSize = 1, PackageSizeUnitId = kilogramId }, [], ingredientCategories.ElementAt(0).Id, [], []);
        Ingredient carrot = Ingredient.Create("Carrot", new() { IngredientUnit = gramId, IngredientAmount = 100 }, gramId, new IngredientPackage { PackageUnitId = kilogramId, PackageSize = 1, PackageSizeUnitId = kilogramId }, [], ingredientCategories.ElementAt(0).Id, [], []);
        Ingredient onion = Ingredient.Create("Onion", new() { IngredientUnit = gramId, IngredientAmount = 50 }, gramId, new IngredientPackage { PackageUnitId = kilogramId, PackageSize = 1, PackageSizeUnitId = kilogramId }, [], ingredientCategories.ElementAt(0).Id, [], []);
        Ingredient water = Ingredient.Create("Water", new() { IngredientUnit = mililiterId, IngredientAmount = 1000 }, mililiterId, new IngredientPackage { PackageUnitId = literId, PackageSize = 1, PackageSizeUnitId = literId }, [], ingredientCategories.ElementAt(0).Id, [], []);
        Ingredient flour = Ingredient.Create("Flour", new() { IngredientUnit = gramId, IngredientAmount = 100 }, gramId, new IngredientPackage { PackageUnitId = kilogramId, PackageSize = 1, PackageSizeUnitId = kilogramId }, [new() { UnitToConvertId = cupId, Ratio = 120 }], ingredientCategories.ElementAt(0).Id, [], []);

        dbContext.Ingredients.AddRange([wholeChicken, carrot, onion, water, flour]);
        await dbContext.SaveChangesAsync();

        // Chicken broth recipe (nested recipe scenario)
        Recipe chickenBrothRecipe = Recipe.Create(
            "Chicken broth",
            RecipeAmount.Create(1.5d, literId.Value), // 1.5 L
            5, // servings
            RecipeDifficulty.Easy,
            new() { IngredientUnit = mililiterId },
            [
                RecipeIngredient.Create(wholeChicken.Id.Value, kilogramId.Value, 1d),
                RecipeIngredient.Create(carrot.Id.Value, gramId.Value, 100d),
                RecipeIngredient.Create(onion.Id.Value, gramId.Value, 50d),
                RecipeIngredient.Create(water.Id.Value, literId.Value, 2d)
            ],
            [
                RecipeSection.Create(1, [RecipeStep.Create(1, "Combine ingredients and simmer", null)])
            ],
            [],
            null!
        );

        // Simple tomato mix recipe (no nested ingredients)
        Ingredient cannedTomatoes = dbContext.Ingredients.First(i => i.Name == "Canned tomatoes");
        Ingredient salt = dbContext.Ingredients.First(i => i.Name == "Salt");
        Recipe simpleTomatoMixRecipe = Recipe.Create(
            "Simple tomato mix",
            RecipeAmount.Create(1d, canId.Value),
            2,
            RecipeDifficulty.Easy,
            new() { IngredientUnit = canId },
            [
                RecipeIngredient.Create(cannedTomatoes.Id.Value, canId.Value, 1d),
                RecipeIngredient.Create(salt.Id.Value, gramId.Value, 5d)
            ],
            [
                RecipeSection.Create(1, [RecipeStep.Create(1, "Mix tomatoes and salt", null)])
            ],
            [],
            null!
        );

        Recipe complexTomatoMixRecipe = Recipe.Create(
            "Complex tomato mix",
            RecipeAmount.Create(1d, canId.Value),
            2,
            RecipeDifficulty.Easy,
            new() { IngredientUnit = canId },
            [
                RecipeIngredient.Create(cannedTomatoes.Id.Value, canId.Value, 1.5d),
                RecipeIngredient.Create(salt.Id.Value, gramId.Value, 5d),
                RecipeIngredient.Create(onion.Id.Value, gramId.Value, 100)
            ],
            [
                RecipeSection.Create(1, [RecipeStep.Create(1, "Mix tomatoes and salt", null)])
            ],
            [],
            null!
        );

        Recipe pizzaDough = Recipe.Create(
            "Pizza dough",
            RecipeAmount.Create(1d, unitId.Value),
            2,
            RecipeDifficulty.Medium,
            new() { IngredientUnit = unitId },
            [
                RecipeIngredient.Create(flour.Id.Value, gramId.Value, 300d),
                RecipeIngredient.Create(water.Id.Value, mililiterId.Value, 200d),
                RecipeIngredient.Create(salt.Id.Value, gramId.Value, 5d)
            ],
            [
                RecipeSection.Create(1, [RecipeStep.Create(1, "Mix ingredients and knead dough", null)])
            ],
            [],
            null!
        );

        Recipe burgerBuns = Recipe.Create(
            "Burger buns",
            RecipeAmount.Create(1d, unitId.Value),
            4,
            RecipeDifficulty.Medium,
            new() { IngredientUnit = unitId },
            [
                RecipeIngredient.Create(flour.Id.Value, cupId.Value, 4),
                RecipeIngredient.Create(water.Id.Value, mililiterId.Value, 250d),
                RecipeIngredient.Create(salt.Id.Value, gramId.Value, 5d)
            ],
            [
                RecipeSection.Create(1, [RecipeStep.Create(1, "Mix ingredients and shape buns", null)])
            ],
            [],
            null!
        );

        dbContext.Recipes.AddRange([chickenBrothRecipe, simpleTomatoMixRecipe, complexTomatoMixRecipe, pizzaDough, burgerBuns]);
        await dbContext.SaveChangesAsync();

        // Add chicken broth as an ingredient linked to its recipe (nested recipe scenario)
        Ingredient chickenBrothIngredient = Ingredient.Create(
            "Chicken broth",
            new() { IngredientUnit = literId },
            literId,
            new IngredientPackage { PackageUnitId = literId, PackageSize = 1, PackageSizeUnitId = literId },
            [],
            null!,
            [],
            [chickenBrothRecipe.Id]
        );
        dbContext.Ingredients.Add(chickenBrothIngredient);
        await dbContext.SaveChangesAsync();

        Recipe mexicanSoup = Recipe.Create(
            "Mexican soup",
            RecipeAmount.Create(4d, literId.Value),
            4,
            RecipeDifficulty.Medium,
            new() { IngredientUnit = literId },
            [
                RecipeIngredient.Create(chickenBrothIngredient.Id.Value, literId.Value, 1.5d),
                RecipeIngredient.Create(cannedTomatoes.Id.Value, canId.Value, 2d),
                RecipeIngredient.Create(salt.Id.Value, gramId.Value, 10d),
                RecipeIngredient.Create(onion.Id.Value, gramId.Value, 150d),
                RecipeIngredient.Create(carrot.Id.Value, gramId.Value, 200d)
            ],
            [
                RecipeSection.Create(1, [RecipeStep.Create(1, "Combine all ingredients and cook", null)])
            ],
            [],
            null!
        );

        dbContext.Recipes.Add(mexicanSoup);
        await dbContext.SaveChangesAsync();
    }
}
