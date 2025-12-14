using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes;

public static class GetRecipesForMealPlan
{
    public sealed record Request(string RecipeName, int NumberOfRecipesToLoad);

    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates);

    public sealed record Response(Guid Id, string Title, string? ImageUrl, NutritionalValueDto NutritionalValues);

    [GroupEndpoint("Recipes")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/mealplan", Handler)
                     .WithName("GetRecipesForMealPlan")
                     .WithDescription("Get recipes for meal plan");
        }
    }

    internal static async Task<Results<Ok<IEnumerable<Response>>, BadRequest>> Handler([AsParameters] Request request, [FromServices] IAppDbContext dbContext, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        IEnumerable<Recipe> recipes = await dbContext.Recipes.AsNoTracking()
                                                             .Where(recipe => recipe.Title.Contains(request.RecipeName) && recipe.CreatedBy == currentUser.Id)
                                                             .Take(request.NumberOfRecipesToLoad)
                                                             .ToListAsync(cancellationToken);

        return TypedResults.Ok(recipes.Select(MapToResponse));
    }

    private static Response MapToResponse(Recipe recipe)
    {
        NutritionalValueDto nutritionalValues = new
        (
            recipe.CalculateCaloriesForSingleServing(),
            recipe.CalculateProteinsForSingleServing(),
            recipe.CalculateFatsForSingleServing(),
            recipe.CalculateProteinsForSingleServing()
        );

        return new Response(recipe.Id.Value, recipe.Title, recipe.ImageURL, nutritionalValues);
    }
}
