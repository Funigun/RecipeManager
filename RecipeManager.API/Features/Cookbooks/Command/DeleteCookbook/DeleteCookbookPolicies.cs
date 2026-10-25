using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Cookbooks.Command.DeleteCookbook;

public sealed class DeleteCookbookAuthorizationPolicy(ICurrentUser currentUser, IAppDbContext dbContext)
    : IAuthorizationPolicy<DeleteCookbookCommand>
{
    public async Task<bool> IsAuthorized(DeleteCookbookCommand command)
    {
        CookbookId id = new(command.CookbookId);
        return await dbContext.Cookbooks.AnyAsync(
            cookbook => cookbook.Id == id && cookbook.CreatedBy == currentUser.Id);
    }
}
