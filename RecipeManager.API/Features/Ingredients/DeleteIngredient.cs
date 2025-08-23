using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients;

public static class DeleteIngredient
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed class AuthorizationPolicy(IAppDbContext dbContext, ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            return await dbContext.Ingredients.AnyAsync(ingredient => ingredient.Id == new IngredientId(request.Id) &&
                                                        ingredient.CreatedBy == currentUser.Id);
        }
    }

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Request>("/{ingredientId}", Handler)
                     .WithName("DeleteIngredient")
                     .WithDescription("Deletes an ingredient");
        }
    }

    public static async Task<IResult> Handler(Request ingredientId, IAppDbContext dbContext, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        IngredientId id = new(ingredientId.Id);

        Ingredient? ingredient = await dbContext.Ingredients.FirstAsync(ingredient => ingredient.Id == id && ingredient.CreatedBy == currentUser.Id, cancellationToken)
                              ?? throw new EntityNotFoundException<Ingredient, IngredientId>(id);

        dbContext.Ingredients.Remove(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
