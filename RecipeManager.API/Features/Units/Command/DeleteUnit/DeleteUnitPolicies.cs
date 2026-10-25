using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units.Command.DeleteUnit;

public sealed class DeleteUnitAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<DeleteUnitCommand>
{
    public Task<bool> IsAuthorized(DeleteUnitCommand command)
    {
        return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
    }
}
