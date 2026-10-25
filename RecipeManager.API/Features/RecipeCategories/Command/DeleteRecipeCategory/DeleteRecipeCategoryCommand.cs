using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.RecipeCategories.Command.DeleteRecipeCategory;

public sealed record DeleteRecipeCategoryCommand(Guid CategoryId);

public sealed class DeleteRecipeCategoryCommandHandler(
    IAppDbContext dbContext,
    IAuthorizationPolicy<DeleteRecipeCategoryCommand> authorizationPolicy)
{
    public async Task<Result<RecipeCategoryId>> Handle(
        DeleteRecipeCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        RecipeCategoryId id = new(command.CategoryId);
        RecipeCategory? category = await dbContext.RecipeCategories
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
        {
            return new NotFoundError();
        }

        dbContext.RecipeCategories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
