using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.IngredientCategories.Command.DeleteIngredientCategory;

public sealed class DeleteIngredientCategoryAuthorizationPolicy(ICurrentUser currentUser)
    : IAuthorizationPolicy<DeleteIngredientCategoryCommand>
{
    public Task<bool> IsAuthorized(DeleteIngredientCategoryCommand command) =>
        Task.FromResult(currentUser.HasRole(UserRoles.Admin));
}
