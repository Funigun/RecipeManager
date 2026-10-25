using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Cookbooks.Command.DeleteCookbook;

public sealed record DeleteCookbookCommand(Guid CookbookId);

public sealed class DeleteCookbookCommandHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IAuthorizationPolicy<DeleteCookbookCommand> authorizationPolicy)
{
    public async Task<Result<CookbookId>> Handle(DeleteCookbookCommand command, CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        CookbookId id = new(command.CookbookId);
        Cookbook? cookbook = await dbContext.Cookbooks.FirstOrDefaultAsync(
            item => item.Id == id && item.CreatedBy == currentUser.Id,
            cancellationToken);
        if (cookbook is null)
        {
            return new NotFoundError();
        }

        dbContext.Cookbooks.Remove(cookbook);
        await dbContext.SaveChangesAsync(cancellationToken);

        return cookbook.Id;
    }
}
