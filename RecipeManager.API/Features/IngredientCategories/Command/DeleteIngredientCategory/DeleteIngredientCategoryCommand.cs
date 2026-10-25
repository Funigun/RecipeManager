using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.IngredientCategories.Command.DeleteIngredientCategory;

public sealed record DeleteIngredientCategoryCommand(Guid CategoryId);

public sealed class DeleteIngredientCategoryCommandHandler(
    IAppDbContext dbContext,
    IAuthorizationPolicy<DeleteIngredientCategoryCommand> authorizationPolicy)
{
    public async Task<Result<IngredientCategoryId>> Handle(
        DeleteIngredientCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        IngredientCategoryId id = new(command.CategoryId);
        IngredientCategory? category = await dbContext.IngredientCategories
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
        {
            return new NotFoundError();
        }

        dbContext.IngredientCategories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
