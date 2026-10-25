using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.RecipeCategories.Command.DeleteRecipeCategory;

public sealed class DeleteRecipeCategoryAuthorizationPolicy(ICurrentUser currentUser)
    : IAuthorizationPolicy<DeleteRecipeCategoryCommand>
{
    public Task<bool> IsAuthorized(DeleteRecipeCategoryCommand command) =>
        Task.FromResult(currentUser.HasRole(UserRoles.Admin));
}
