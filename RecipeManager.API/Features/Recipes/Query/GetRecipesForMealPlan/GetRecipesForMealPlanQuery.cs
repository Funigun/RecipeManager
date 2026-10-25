using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Recipes.Query.GetRecipesForMealPlan;

public sealed record GetRecipesForMealPlanParameters(string RecipeName, int NumberOfRecipesToLoad);
public sealed record GetRecipesForMealPlanQuery(GetRecipesForMealPlanParameters Parameters);
internal sealed record GetRecipesForMealPlanProjection(Recipe Recipe);
public sealed record GetRecipesForMealPlanNutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates);
public sealed record GetRecipesForMealPlanResponse(Guid Id, string Title, string? ImageUrl, GetRecipesForMealPlanNutritionalValueDto NutritionalValues);

public sealed class GetRecipesForMealPlanQueryHandler(IAppDbContext dbContext, ICurrentUser currentUser)
{
    public async Task<IEnumerable<GetRecipesForMealPlanResponse>> Handle(
        GetRecipesForMealPlanQuery query,
        CancellationToken cancellationToken)
    {
        GetRecipesForMealPlanParameters parameters = query.Parameters;
        List<Recipe> recipes = await dbContext.Recipes.AsNoTracking()
            .Where(recipe => recipe.Title.Contains(parameters.RecipeName) && recipe.CreatedBy == currentUser.Id)
            .Take(parameters.NumberOfRecipesToLoad)
            .ToListAsync(cancellationToken);
        return recipes.Select(MapToResponse);
    }

    private static GetRecipesForMealPlanResponse MapToResponse(Recipe recipe)
    {
        GetRecipesForMealPlanNutritionalValueDto nutritionalValues = new(
            recipe.CalculateCaloriesForSingleServing(),
            recipe.CalculateProteinsForSingleServing(),
            recipe.CalculateFatsForSingleServing(),
            recipe.CalculateProteinsForSingleServing());
        return new GetRecipesForMealPlanResponse(
            recipe.Id.Value, recipe.Title, recipe.ImageURL, nutritionalValues);
    }
}
