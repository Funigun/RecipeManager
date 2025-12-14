using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes;

public static class GetRecipeForDropdownModel
{
    public sealed record Response(Guid Id, string Title, string? ImageUrl);

    [GroupEndpoint("Recipes")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/dropdown", Handler)
                     .WithName("GetRecipeForDropdownModel")
                     .WithDescription("Get a recipe for dropdown model");
        }
    }

    internal static async Task<Results<Ok<IEnumerable<Response>>, BadRequest>> Handler([FromQuery] string recipeName, [FromQuery] int numberOfRecipesToLoad, [FromServices] IAppDbContext dbContext, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        IEnumerable<Recipe> recipes = await dbContext.Recipes.AsNoTracking()
                                                             .Where(recipe => recipe.Title.Contains(recipeName) && recipe.CreatedBy == currentUser.Id)
                                                             .Take(numberOfRecipesToLoad)
                                                             .ToListAsync(cancellationToken);

        return TypedResults.Ok(recipes.Select(MapToResponse));
    }

    private static Response MapToResponse(Recipe recipe)
    {
        return new Response(recipe.Id.Value, recipe.Title, recipe.ImageURL);
    }
}
