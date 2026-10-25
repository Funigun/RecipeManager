using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Ingredients.Command.DeleteIngredient;

public sealed record DeleteIngredientCommand(Guid IngredientId);

public sealed class DeleteIngredientCommandHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IAuthorizationPolicy<DeleteIngredientCommand> authorizationPolicy)
{
    public async Task<Result<IngredientId>> Handle(
        DeleteIngredientCommand command,
        CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        IngredientId id = new(command.IngredientId);
        Ingredient? ingredient = await dbContext.Ingredients.FirstOrDefaultAsync(
            item => item.Id == id && item.CreatedBy == currentUser.Id,
            cancellationToken);
        if (ingredient is null)
        {
            return new NotFoundError();
        }

        dbContext.Ingredients.Remove(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.Id;
    }
}
