using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Validation;

namespace RecipeManager.Api.Features.Ingredients.Command.GenerateShoppingList;

public sealed record GenerateShoppingListRecipeDto(Guid Id);

public sealed record GenerateShoppingListRequest(IEnumerable<GenerateShoppingListRecipeDto> Recipes);

public sealed record GenerateShoppingListIngredientDto(string Name, double Quantity, string Unit, string Category);

public sealed record GenerateShoppingListResponse(Dictionary<string, List<GenerateShoppingListIngredientDto>> Ingredients);

public sealed record GenerateShoppingListCommand(GenerateShoppingListRequest Request);

public sealed class GenerateShoppingListCommandHandler(
    IAppDbContext dbContext,
    IValidator<GenerateShoppingListRequest> validator)
{
    private sealed record UnitData(UnitId Id, UnitId? PrimaryUnit, int ConversionFactor, string Name, string? ShortName);

    private sealed class RecipeDao
    {
        public Guid Id { get; init; }

        public List<RecipeIngredientDao> Ingredients { get; set; } = [];
    }

    private sealed class RecipeIngredientDao
    {
        public Guid IngredientId { get; init; }

        public double Amount { get; set; }

        public Guid UnitId { get; set; }

        public string UnitDisplayName { get; set; } = string.Empty;

        public RecipeDao? IngredientRecipe { get; set; }
    }

    public async Task<Result<GenerateShoppingListResponse>> Handle(
        GenerateShoppingListCommand command,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<GenerateShoppingListRequest>.ToValidationError(validationResult);
        }

        GenerateShoppingListRequest request = command.Request;
        Dictionary<UnitId, UnitData> units = await GetUnits(dbContext, cancellationToken);

        IEnumerable<Recipe> recipes = await GetRecipes(
            request.Recipes.Select(recipe => recipe.Id),
            dbContext,
            cancellationToken);
        IEnumerable<Recipe> nestedRecipes = await GetNestedRecipes(recipes, dbContext, cancellationToken);

        IEnumerable<RecipeIngredient> recipeIngredients = recipes.SelectMany(recipe => recipe.Ingredients)
            .Concat(nestedRecipes.SelectMany(recipe => recipe.Ingredients));
        IEnumerable<IngredientId> ingredientIds = recipeIngredients.Select(item => item.IngredientId).Distinct();

        Dictionary<IngredientId, Ingredient> ingredients =
            (await GetIngredients(ingredientIds, dbContext, cancellationToken)).ToDictionary(ingredient => ingredient.Id);
        Dictionary<IngredientCategoryId, IngredientCategory> ingredientCategories =
            (await GetIngredientCategories(ingredients.Values, dbContext, cancellationToken))
            .ToDictionary(category => category.Id);

        IEnumerable<RecipeDao> selectedRecipes = PrepareRecipes(
            request,
            recipes,
            ingredients,
            nestedRecipes.ToDictionary(recipe => recipe.Id));
        IEnumerable<RecipeIngredientDao> ingredientsToProcess = selectedRecipes.SelectMany(recipe => recipe.Ingredients);

        foreach (RecipeIngredientDao recipeIngredient in ingredientsToProcess)
        {
            Ingredient ingredient = ingredients[recipeIngredient.IngredientId];
            UnitData recipeIngredientUnit = units[recipeIngredient.UnitId];

            recipeIngredient.Amount = recipeIngredient.UnitId == ingredient.BaseUnit.Value
                ? recipeIngredient.Amount
                : recipeIngredientUnit.PrimaryUnit == ingredient.BaseUnit
                    ? recipeIngredient.Amount * recipeIngredientUnit.ConversionFactor
                    : recipeIngredient.Amount * ingredient.IngredientUnitConvertions.First(
                        conversion => conversion.UnitToConvertId.Value == recipeIngredient.UnitId).Ratio;
        }

        IEnumerable<RecipeIngredientDao> ingredientsByBaseUnitAmount = ingredientsToProcess
            .GroupBy(item => item.IngredientId)
            .Select(group =>
            {
                Ingredient ingredient = ingredients[group.Key];
                UnitData ingredientBaseUnit = units[
                    ingredient.BaseUnit ?? throw new InvalidOperationException("Ingredient base unit is null")];
                return new RecipeIngredientDao
                {
                    IngredientId = group.Key,
                    Amount = group.Sum(item => item.Amount),
                    UnitId = ingredientBaseUnit.Id
                };
            })
            .ToList();

        foreach (RecipeIngredientDao recipeIngredient in ingredientsByBaseUnitAmount)
        {
            Ingredient ingredient = ingredients[recipeIngredient.IngredientId];
            UnitData ingredientPackageUnit = units[ingredient.IngredientPackage.PackageUnitId];
            UnitData ingredientPackageSizeUnit = units[ingredient.IngredientPackage.PackageSizeUnitId];

            recipeIngredient.Amount = ingredient.IngredientPackage.PackageSizeUnitId == ingredient.BaseUnit
                ? recipeIngredient.Amount
                : ingredientPackageSizeUnit.PrimaryUnit == ingredient.BaseUnit
                    ? recipeIngredient.Amount / ingredientPackageSizeUnit.ConversionFactor
                    : recipeIngredient.Amount * ingredient.IngredientUnitConvertions.First(
                        conversion => conversion.UnitToConvertId == ingredient.IngredientPackage.PackageSizeUnitId).Ratio;

            recipeIngredient.Amount = Math.Ceiling(recipeIngredient.Amount / ingredient.IngredientPackage.PackageSize);

            recipeIngredient.UnitDisplayName = ingredientPackageUnit.Id == ingredientPackageSizeUnit.Id
                ? ingredientPackageUnit.ShortName ?? ingredientPackageUnit.Name
                : $"{ingredientPackageSizeUnit.Name} ({ingredientPackageUnit.ShortName ?? ingredientPackageUnit.Name})";
        }

        Dictionary<string, List<GenerateShoppingListIngredientDto>> groupedIngredients =
            ingredientsByBaseUnitAmount
                .Select(group => new GenerateShoppingListIngredientDto(
                    ingredients[group.IngredientId].Name,
                    Math.Round(group.Amount, 2),
                    group.UnitDisplayName,
                    ingredientCategories[ingredients[group.IngredientId].GetShoppingListGroupId()].Name))
                .GroupBy(ingredient => ingredient.Category)
                .ToDictionary(group => group.Key, group => group.ToList());

        return new GenerateShoppingListResponse(groupedIngredients);
    }

    private static async Task<IEnumerable<Recipe>> GetRecipes(
        IEnumerable<Guid> recipeIds,
        IAppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Recipes.AsNoTracking()
            .AsSplitQuery()
            .Include(recipe => recipe.Ingredients)
            .Where(recipe => recipeIds.Contains(recipe.Id))
            .ToListAsync(cancellationToken);
    }

    private static async Task<IEnumerable<Recipe>> GetNestedRecipes(
        IEnumerable<Recipe> recipes,
        IAppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        List<Recipe> results = [];

        HashSet<IngredientId> recipeIngredientIds = recipes.SelectMany(recipe => recipe.Ingredients)
            .Select(ingredient => ingredient.IngredientId)
            .ToHashSet();

        IEnumerable<Ingredient> ingredientsWithRecipe = await dbContext.Ingredients.AsNoTracking()
            .AsSplitQuery()
            .Include(ingredient => ingredient.Recipes)
            .Where(ingredient => recipeIngredientIds.Contains(ingredient.Id) && ingredient.Recipes.Any())
            .ToListAsync(cancellationToken);

        if (ingredientsWithRecipe.Any())
        {
            results = (await GetRecipes(
                ingredientsWithRecipe.SelectMany(ingredient => ingredient.Recipes).Select(recipe => recipe.Value),
                dbContext,
                cancellationToken)).ToList();
            results.AddRange((await GetNestedRecipes(results, dbContext, cancellationToken)).ToList());
        }

        return results;
    }

    private static async Task<Dictionary<UnitId, UnitData>> GetUnits(
        IAppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Units.AsNoTracking()
            .Select(unit => new UnitData(unit.Id, unit.PrimaryUnit, unit.ConversionFactor, unit.Name, unit.ShortName))
            .ToDictionaryAsync(unit => unit.Id, cancellationToken);
    }

    private static async Task<IEnumerable<Ingredient>> GetIngredients(
        IEnumerable<IngredientId> ingredientIds,
        IAppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Ingredients.AsNoTracking()
            .AsSplitQuery()
            .Include(ingredient => ingredient.Categories)
            .Include(ingredient => ingredient.IngredientUnitConvertions)
            .Include(ingredient => ingredient.IngredientPackage)
            .Where(ingredient => ingredientIds.Contains(ingredient.Id))
            .ToHashSetAsync(cancellationToken);
    }

    private static async Task<IEnumerable<IngredientCategory>> GetIngredientCategories(
        IEnumerable<Ingredient> ingredients,
        IAppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        IEnumerable<IngredientCategoryId> ingredientCategoryIds = ingredients
            .Select(ingredient => ingredient.GetShoppingListGroupId())
            .Where(id => id.Value != Guid.Empty)
            .Distinct();

        return await dbContext.IngredientCategories.AsNoTracking()
            .Where(category => ingredientCategoryIds.Contains(category.Id))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private static IEnumerable<RecipeDao> PrepareRecipes(
        GenerateShoppingListRequest request,
        IEnumerable<Recipe> recipes,
        Dictionary<IngredientId, Ingredient> ingredients,
        Dictionary<RecipeId, Recipe> nestedRecipes)
    {
        List<RecipeDao> result = [];

        foreach (GenerateShoppingListRecipeDto recipeDto in request.Recipes)
        {
            Recipe recipe = recipes.First(item => item.Id.Value == recipeDto.Id);
            result.Add(new RecipeDao
            {
                Id = recipe.Id.Value,
                Ingredients = GetRecipeIngredients(recipe, ingredients, nestedRecipes).ToList()
            });
        }

        return result;
    }

    private static IEnumerable<RecipeIngredientDao> GetRecipeIngredients(
        Recipe recipe,
        Dictionary<IngredientId, Ingredient> ingredients,
        Dictionary<RecipeId, Recipe> nestedRecipes,
        double nestedRecipeYieldFactor = 1d)
    {
        List<RecipeIngredientDao> results = [];

        foreach (RecipeIngredient recipeIngredient in recipe.Ingredients)
        {
            Ingredient ingredient = ingredients[recipeIngredient.IngredientId];
            if (ingredient.Recipes.Any())
            {
                Recipe nestedRecipe = nestedRecipes[ingredient.Recipes.First()];
                double childRecipeYieldFactor = nestedRecipe.Amount.Amount * nestedRecipeYieldFactor /
                                                recipeIngredient.Amount;
                results.AddRange(GetRecipeIngredients(
                    nestedRecipe,
                    ingredients,
                    nestedRecipes,
                    childRecipeYieldFactor));
            }
            else
            {
                results.Add(new RecipeIngredientDao
                {
                    IngredientId = recipeIngredient.IngredientId,
                    Amount = recipeIngredient.Amount * nestedRecipeYieldFactor,
                    UnitId = recipeIngredient.UnitId
                });
            }
        }

        return results;
    }
}
