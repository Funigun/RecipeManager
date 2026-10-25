using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Recipes.Command.DeleteRecipe;

public sealed record DeleteRecipeCommand(Guid RecipeId);
internal sealed record DeleteRecipeProjection(Recipe Recipe);
public sealed record DeleteRecipeResponse(RecipeId Id);

public sealed class DeleteRecipeCommandHandler(
    IAppDbContext dbContext,
    IAuthorizationPolicy<DeleteRecipeCommand> authorizationPolicy,
    ICurrentUser currentUser)
{
    public async Task<Result<DeleteRecipeResponse>> Handle(DeleteRecipeCommand command, CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        RecipeId id = new(command.RecipeId);
        Recipe? recipe = await dbContext.Recipes.FirstOrDefaultAsync(
            item => item.Id == id && item.CreatedBy == currentUser.Id,
            cancellationToken);
        if (recipe is null)
        {
            return new NotFoundError();
        }

        dbContext.Recipes.Remove(recipe);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteRecipeResponse(recipe.Id);
    }
}
