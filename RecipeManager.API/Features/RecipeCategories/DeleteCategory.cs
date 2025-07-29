using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.RecipeCategories;

public static class DeleteCategory
{
    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Guid>
    {
        public Task<bool> IsAuthorized(Guid request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    [GroupEndpoint("RecipeCategories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Guid>("/{categoryId}", Handler)
                     .WithName("DeleteRecipeCategory")
                     .WithDescription("Deletes a recipe category");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(Guid unitId, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        RecipeCategoryId id = new(unitId);

        RecipeCategory? category = await dbContext.RecipeCategories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken)
                                ?? throw new EntityNotFoundException<RecipeCategory, RecipeCategoryId>(id);

        dbContext.RecipeCategories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
