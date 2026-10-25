using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units.Command.DeleteUnit;

public sealed record DeleteUnitCommand(Guid UnitId);

public sealed class DeleteUnitCommandHandler(IAppDbContext dbContext, IAuthorizationPolicy<DeleteUnitCommand> authorizationPolicy)
{
    public async Task<Result<UnitId>> Handle(DeleteUnitCommand command, CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        UnitId id = new(command.UnitId);
        Unit? unit = await dbContext.Units.FirstOrDefaultAsync(unit => unit.Id == id, cancellationToken);
        if (unit is null)
        {
            return new NotFoundError();
        }

        dbContext.Units.Remove(unit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return unit.Id;
    }
}
