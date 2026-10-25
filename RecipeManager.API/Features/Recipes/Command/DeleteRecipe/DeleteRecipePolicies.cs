using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Recipes.Command.DeleteRecipe;

public sealed class DeleteRecipeAuthorizationPolicy(IAppDbContext dbContext, ICurrentUser currentUser)
    : IAuthorizationPolicy<DeleteRecipeCommand>
{
    public Task<bool> IsAuthorized(DeleteRecipeCommand command) =>
        dbContext.Recipes.AnyAsync(recipe =>
            recipe.Id == new RecipeId(command.RecipeId) && recipe.CreatedBy == currentUser.Id);
}
