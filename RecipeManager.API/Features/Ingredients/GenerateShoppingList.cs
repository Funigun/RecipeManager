using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients;

public static class GenerateShoppingList
{
    public sealed record Request(IEnumerable<Guid> RecipeIds);

    public sealed record IngredientDto(string Name, double Quantity, string Unit);

    public sealed record Response(Dictionary<string, IEnumerable<IngredientDto>> Ingredients);

    public sealed class ValidationPolicy : AbstractValidator<Request>
    {
        public ValidationPolicy(IAppDbContext dbContext)
        {
            RuleFor(request => request.RecipeIds)
                .NotEmpty()
                    .WithMessage("At least one recipe ID must be provided.")

                .MustAsync(async (recipeIds, cancellationToken) =>
                {
                    int numberOfExistingRecipes= await dbContext.Recipes
                        .Where(recipe => recipeIds.Contains(recipe.Id.Value))
                        .Select(recipe => recipe.Id.Value)
                        .CountAsync(cancellationToken);

                    return numberOfExistingRecipes == recipeIds.Distinct().Count();
                }).WithMessage("Some of the provided recipe IDs do not exist.");
        }
    }

    [EndpointGroupName("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedGet<Response>("generate-shopping-list", Handler)
                     .WithName("GenerateShoppingList")
                     .WithDescription("Generates a shopping list based on the provided recipe IDs.");
        }
    }

    private static async Task<Results<Ok<Response>, BadRequest>> Handler([FromBody] Request request, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Dictionary<string, IEnumerable<IngredientDto>> results = [];

        List<Recipe> recipes = await dbContext.Recipes.AsNoTracking()
                                                      .AsSplitQuery()
                                                      .Include(recipe => recipe.Ingredients)
                                                      .Where(recipe => request.RecipeIds.Contains(recipe.Id))
                                                      .ToListAsync(cancellationToken);

        IEnumerable<RecipeIngredient> recipeIngredients = recipes.SelectMany(recipe => recipe.Ingredients);
        IEnumerable<IngredientId>? ingredientIds = recipeIngredients.Select(i => i.IngredientId).Distinct();
        IEnumerable<UnitId>? unitIds = recipeIngredients.Select(i => i.UnitId).Distinct();

        HashSet<Unit> units = await dbContext.Units.AsNoTracking()
                                                   .Where(unit => unitIds.Contains(unit.Id))
                                                   .ToHashSetAsync(cancellationToken);

        HashSet<Ingredient> ingredients = await dbContext.Ingredients.AsNoTracking()
                                                                     .AsSplitQuery()
                                                                     .Include(ingredient => ingredient.Categories)
                                                                     .Where(ingredient => ingredientIds.Contains(ingredient.Id))
                                                                     .ToHashSetAsync(cancellationToken);

        IEnumerable<IngredientCategoryId> ingredientCategoryIds = ingredients.Select(ingredient => ingredient.GetShoppingListGroupId())
                                                                             .Where(id => id.Value != Guid.Empty)
                                                                             .Distinct();

        IEnumerable<IngredientCategory> ingredientCategories = await dbContext.IngredientCategories.AsNoTracking()
                                                                                                   .Where(category => ingredientCategoryIds.Contains(category.Id))
                                                                                                   .Distinct()
                                                                                                   .ToListAsync(cancellationToken);



        IEnumerable<IGrouping<IngredientCategoryId, Ingredient>>? groupedIngredients = ingredients.GroupBy(ingredient => ingredient.GetShoppingListGroupId());

        foreach (IGrouping<IngredientCategoryId, Ingredient> group in groupedIngredients)
        {
            string categoryName = ingredientCategories.FirstOrDefault(category => category.Id == group.Key)?.Name ?? "Other";

            results.Add
            (
                categoryName,
                group.Select(ingredient => MapToIngredientDto(ingredient, recipeIngredients, units))
            );
        }

        return TypedResults.Ok(new Response(results));
    }

    private static IngredientDto MapToIngredientDto(Ingredient ingredient, IEnumerable<RecipeIngredient> recipeIngredients, HashSet<Unit> units)
    {
        RecipeIngredient recipeIngredient = recipeIngredients.First(recipeIngredient => recipeIngredient.IngredientId == ingredient.Id);
        Unit ingredientUnit = units.First(u => u.Id == recipeIngredient.UnitId);

        return new
        (
            ingredient.Name,
            recipeIngredient.Amount,
            ingredientUnit.ShortName ?? ingredientUnit.Name
        );
    }
}
