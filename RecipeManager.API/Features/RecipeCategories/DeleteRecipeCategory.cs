using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.RecipeCategories;

public static class DeleteRecipeCategory
{
    public record struct Request(Guid Value) : IRequestId<Request>
    {
    }

    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public Task<bool> IsAuthorized(Request request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    [GroupEndpoint("RecipeCategories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Request>("/{categoryId}", Handler)
                     .WithName("DeleteRecipeCategory")
                     .WithDescription("Deletes a recipe category");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(Request categoryId, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        RecipeCategoryId id = new(categoryId.Value);

        RecipeCategory? category = await dbContext.RecipeCategories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken)
                                ?? throw new EntityNotFoundException<RecipeCategory, RecipeCategoryId>(id);

        dbContext.RecipeCategories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
