using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients;

public static class GenerateShoppingList
{
    private sealed class RecipeDao()
    {
        public Guid Id { get; init; }

        public List<RecipeIngredientDao> Ingredients { get; set; } = [];
    }

    private sealed class RecipeIngredientDao()
    {
        public Guid IngredientId { get; init; }

        public double Amount { get; set; }

        public Guid UnitId { get; set; }

        public string UnitDisplayName { get; set; }

        public RecipeDao? IngredientRecipe { get; set; }
    }

    public sealed record RecipeDto(Guid Id, int NumberOfServings);

    public sealed record Request(IEnumerable<RecipeDto> Recipes);

    public sealed record IngredientDto(string Name, double Quantity, string Unit, string Category);

    public sealed record Response(Dictionary<string, List<IngredientDto>> Ingredients);

    public sealed class ValidationPolicy : AbstractValidator<Request>
    {
        public ValidationPolicy(IAppDbContext dbContext)
        {
            RuleFor(request => request.Recipes)
                .NotEmpty()
                    .WithMessage("At least one recipe ID must be provided.")

                .MustAsync(async (recipes, cancellationToken) =>
                {
                    IEnumerable<RecipeId> recipeIds = recipes.Select(r => new RecipeId(r.Id));

                    int numberOfExistingRecipes = await dbContext.Recipes
                        .Where(recipe => recipeIds.Contains(recipe.Id))
                        .Select(recipe => recipe.Id.Value)
                        .CountAsync(cancellationToken);

                    return numberOfExistingRecipes == recipeIds.Distinct().Count();
                }).WithMessage("Some of the provided recipe IDs do not exist.");
        }
    }

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardValidatedPut<Request>("generate-shopping-list", Handler)
                     .WithName("GenerateShoppingList")
                     .WithDescription("Generates a shopping list based on the provided recipe IDs.");
        }
    }

    public static async Task<Results<Ok<Response>, BadRequest>> Handler(Request request, [FromServices] IAppDbContext dbContext, [FromServices] IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        Dictionary<UnitId, Unit> units = await GetUnits(unitOfWork, cancellationToken);

        IEnumerable<Recipe> recipes = await GetRecipes(request.Recipes.Select(r => r.Id), dbContext, cancellationToken);
        IEnumerable<Recipe> nestedRecipes = await GetNestedRecipes(recipes, dbContext, cancellationToken);

        IEnumerable<RecipeIngredient> recipeIngredients = recipes.SelectMany(recipe => recipe.Ingredients).Concat(nestedRecipes.SelectMany(recipe => recipe.Ingredients));
        IEnumerable<IngredientId> ingredientIds = recipeIngredients.Select(i => i.IngredientId).Distinct();

        Dictionary<IngredientId, Ingredient> ingredients = (await GetIngredients(ingredientIds, dbContext, cancellationToken)).ToDictionary(i => i.Id);
        Dictionary<IngredientCategoryId, IngredientCategory> ingredientCategories = (await GetIngredientCategories(ingredients.Values, dbContext, cancellationToken)).ToDictionary(category => category.Id);

        IEnumerable<RecipeDao>? scalledRecipes = PrepareScalledRecipes(request, recipes, ingredients, nestedRecipes.ToDictionary(r => r.Id));
        IEnumerable<RecipeIngredientDao> ingredientsToProcess = scalledRecipes.SelectMany(r => r.Ingredients);

        foreach (RecipeIngredientDao recipeIngredient in ingredientsToProcess)
        {
            Ingredient ingredient = ingredients[recipeIngredient.IngredientId];
            Unit recipeIngredientUnit = units[recipeIngredient.UnitId];

            // Convert recipe ingredient amount to ingredient base unit
            recipeIngredient.Amount = recipeIngredient.UnitId == ingredient.BaseUnit.Value
                                    ? recipeIngredient.Amount
                                    : recipeIngredientUnit.PrimaryUnit == ingredient.BaseUnit
                                      ? recipeIngredient.Amount * recipeIngredientUnit.ConversionFactor
                                      : recipeIngredient.Amount * ingredient.IngredientUnitConvertions.First(conversion => conversion.UnitToConvertId.Value == recipeIngredient.UnitId).Ratio;
        }
        
        IEnumerable<RecipeIngredientDao> ingredientsByBaseUnitAmount = ingredientsToProcess.GroupBy(i => i.IngredientId)
                                                                                           .Select(group =>
                                                                                           {
                                                                                               Ingredient ingredient = ingredients[group.Key];
                                                                                               Unit ingredientBaseUnit = units[ingredient.BaseUnit ?? throw new InvalidOperationException("Ingredient base unit is null")];
                                                                                               return new RecipeIngredientDao
                                                                                                   {
                                                                                                       IngredientId = group.Key,
                                                                                                       Amount = group.Sum(i => i.Amount),
                                                                                                       UnitId = ingredientBaseUnit.Id,
                                                                                                   };
                                                                                           })
                                                                                           .ToList();

        foreach (RecipeIngredientDao recipeIngredient in ingredientsByBaseUnitAmount)
        {
            Ingredient ingredient = ingredients[recipeIngredient.IngredientId];
            Unit ingredientPackageUnit = units[ingredient.IngredientPackage.PackageUnitId];
            Unit ingredientPackageSizeUnit = units[ingredient.IngredientPackage.PackageSizeUnitId];

            // Convert ingredient base amount to ingredient package size unit
            recipeIngredient.Amount = ingredient.IngredientPackage.PackageSizeUnitId == ingredient.BaseUnit
                                    ? recipeIngredient.Amount
                                    : ingredientPackageSizeUnit.PrimaryUnit == ingredient.BaseUnit
                                      ? recipeIngredient.Amount / ingredientPackageSizeUnit.ConversionFactor
                                      : recipeIngredient.Amount * ingredient.IngredientUnitConvertions.First(conversion => conversion.UnitToConvertId == ingredient.IngredientPackage.PackageSizeUnitId).Ratio;

            // Convert to number of packages
            recipeIngredient.Amount = Math.Ceiling(recipeIngredient.Amount / ingredient.IngredientPackage.PackageSize);

            recipeIngredient.UnitDisplayName = ingredientPackageUnit.Id == ingredientPackageSizeUnit.Id
                                             ? ingredientPackageUnit.ShortName ?? ingredientPackageUnit.Name
                                             : $"{ingredientPackageSizeUnit.Name} ({ingredientPackageUnit.ShortName ?? ingredientPackageUnit.Name})";
        }

        Dictionary<string, List<IngredientDto>> groupedIngredients = ingredientsByBaseUnitAmount.Select(group => new IngredientDto
                                                                               (
                                                                                   ingredients[group.IngredientId].Name,
                                                                                   Math.Round(group.Amount, 2),
                                                                                   group.UnitDisplayName,
                                                                                   ingredientCategories[ingredients[group.IngredientId].GetShoppingListGroupId()].Name
                                                                               ))
                                                                               .GroupBy(i => i.Category)
                                                                                   .ToDictionary(g => g.Key, g => g.ToList());

        return TypedResults.Ok(new Response(groupedIngredients));
    }

    private static async Task<IEnumerable<Recipe>> GetRecipes(IEnumerable<Guid> recipeIds, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Recipes.AsNoTracking()
                                      .AsSplitQuery()
                                      .Include(recipe => recipe.Ingredients)
                                      .Where(recipe => recipeIds.Contains(recipe.Id))
                                      .ToListAsync(cancellationToken);
    }

    private static async Task<IEnumerable<Recipe>> GetNestedRecipes(IEnumerable<Recipe> recipes, IAppDbContext dbContext, CancellationToken cancellationToken)
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
            results = (await GetRecipes(ingredientsWithRecipe.SelectMany(i => i.Recipes).Select(r => r.Value), dbContext, cancellationToken)).ToList();
            results.AddRange((await GetNestedRecipes(results, dbContext, cancellationToken)).ToList());
        }

        return results;
    }

    private static async Task<Dictionary<UnitId, Unit>> GetUnits(IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        return (await unitOfWork.Units.GetAllAsync(cancellationToken))
                                    .ToDictionary(unit => unit.Id, unit => unit);
    }

    private static async Task<IEnumerable<Ingredient>> GetIngredients(IEnumerable<IngredientId> ingredientIds, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Ingredients.AsNoTracking()
                                          .AsSplitQuery()
                                          .Include(ingredient => ingredient.Categories)
                                          .Include(ingredient => ingredient.IngredientUnitConvertions)
                                          .Include(ingredient => ingredient.IngredientPackage)
                                          .Where(ingredient => ingredientIds.Contains(ingredient.Id))
                                          .ToHashSetAsync(cancellationToken);
    }

    private static async Task<IEnumerable<IngredientCategory>> GetIngredientCategories(IEnumerable<Ingredient> ingredients, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IEnumerable<IngredientCategoryId> ingredientCategoryIds = ingredients.Select(ingredient => ingredient.GetShoppingListGroupId())
                                                                             .Where(id => id.Value != Guid.Empty)
                                                                             .Distinct();

        return await dbContext.IngredientCategories.AsNoTracking()
                                                   .Where(category => ingredientCategoryIds.Contains(category.Id))
                                                   .Distinct()
                                                   .ToListAsync(cancellationToken);
    }

    private static IEnumerable<RecipeDao> PrepareScalledRecipes(Request request, IEnumerable<Recipe> recipes, Dictionary<IngredientId, Ingredient> ingredients, Dictionary<RecipeId, Recipe> nestedRecipes)
    {
        List<RecipeDao> result = [];

        foreach (RecipeDto recipeDto in request.Recipes)
        {
            Recipe recipe = recipes.First(r => r.Id.Value == recipeDto.Id);

            RecipeDao recipeDao = new()
            {
                Id = recipe.Id.Value,
                Ingredients = GetScalledIngredients(recipeDto, recipe, ingredients, nestedRecipes).ToList()
            };

            result.Add(recipeDao);
        }

        return result;
    }

    private static IEnumerable<RecipeIngredientDao> GetScalledIngredients(RecipeDto recipeDto, Recipe recipe, Dictionary<IngredientId, Ingredient> ingredients, Dictionary<RecipeId, Recipe> nestedRecipes, double scalingFactorByAmount = 0d)
    {
        List<RecipeIngredientDao> results = [];
        double scalingFactorByRecipeAmount = (double)recipeDto.NumberOfServings / recipe.NumberOfServings;
        double finalScalingFactor = scalingFactorByAmount > 0 ? scalingFactorByAmount : scalingFactorByRecipeAmount;

        foreach (RecipeIngredient recipeIngredient in recipe.Ingredients)
        {
            Ingredient ingredient = ingredients[recipeIngredient.IngredientId];
            if (ingredient.Recipes.Any())
            {
                Recipe nestedRecipe = nestedRecipes[ingredient.Recipes.First()];
                double nestedRecipeScalingFactorByAmount = nestedRecipe.Amount.Amount * scalingFactorByRecipeAmount / recipeIngredient.Amount;
                results.AddRange(GetScalledIngredients(recipeDto, nestedRecipe, ingredients, nestedRecipes, nestedRecipeScalingFactorByAmount).ToList());
            }
            else
            {
                results.Add(new RecipeIngredientDao
                {
                    IngredientId = recipeIngredient.IngredientId,
                    Amount = recipeIngredient.Amount * finalScalingFactor,
                    UnitId = recipeIngredient.UnitId
                });
            }
        }

        return results;
    }
}
