using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.IngredientCategories;

public static class DeleteIngredientCategory
{
    public record struct Request(Guid Id) : IRequestId<Request>
    {
    }

    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public Task<bool> IsAuthorized(Request request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Request>(IngredientCategoryGroup.CategoryByIdRoute, Handler)
                     .WithName("DeleteIngredientCategory")
                     .WithDescription("Deletes an ingredient category")
                     .WithTags(IngredientCategoryGroup.Tag)
                     .RequireAuthorization("RecipeManagerPolicy")
                     .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }

    public static async Task<IResult> Handler(Request categoryId, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IngredientCategoryId id = new(categoryId.Id);

        IngredientCategory? category = await dbContext.IngredientCategories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken)
                                  ?? throw new EntityNotFoundException<IngredientCategory, IngredientCategoryId>(id);

        dbContext.IngredientCategories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
