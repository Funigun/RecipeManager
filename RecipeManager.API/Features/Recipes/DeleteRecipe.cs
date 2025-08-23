using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes;

public static class DeleteRecipe
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed class AuthorizationPolicy(IAppDbContext dbContext, ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            return await dbContext.Recipes.AnyAsync(r => r.Id == new RecipeId(request.Id) && r.CreatedBy == currentUser.Id);
        }
    }

    [GroupEndpoint("Recipes")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Request>("{recipeId}", Handler)
                     .WithName("DeleteRecipe")
                     .WithDescription("Delete a recipe by id");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(Request recipeId, [FromServices] IAppDbContext dbContext, [FromServices] ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        RecipeId id = new(recipeId.Id);
        Recipe? recipe = await dbContext.Recipes.FirstAsync(recipe => recipe.Id == id && recipe.CreatedBy == currentUser.Id, cancellationToken);

        if (recipe == null)
        {
            throw new EntityNotFoundException<Recipe, RecipeId>(new RecipeId(id));
        }

        dbContext.Recipes.Remove(recipe);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
