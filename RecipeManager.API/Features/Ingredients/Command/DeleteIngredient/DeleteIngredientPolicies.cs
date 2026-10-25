using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Ingredients.Command.DeleteIngredient;

public sealed class DeleteIngredientAuthorizationPolicy(
    IAppDbContext dbContext,
    ICurrentUser currentUser) : IAuthorizationPolicy<DeleteIngredientCommand>
{
    public Task<bool> IsAuthorized(DeleteIngredientCommand command)
    {
        IngredientId id = new(command.IngredientId);
        return dbContext.Ingredients.AsNoTracking()
            .AnyAsync(ingredient => ingredient.Id == id && ingredient.CreatedBy == currentUser.Id);
    }
}
