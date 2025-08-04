using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.IngredientCategories;

public static class DeleteIngredientCategory
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

    [GroupEndpoint("IngredientCategories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Request>(string.Empty, Handler)
                     .WithName("DeleteIngredientCategory")
                     .WithDescription("Deletes an ingredient category");
        }
    }

    public static async Task<IResult> Handler(Request request, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IngredientCategoryId id = new(request.Value);

        IngredientCategory? category = await dbContext.IngredientCategories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken)
                                  ?? throw new EntityNotFoundException<IngredientCategory, IngredientCategoryId>(id);

        dbContext.IngredientCategories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
