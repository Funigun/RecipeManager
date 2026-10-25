using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;

namespace RecipeManager.Api.Features.Units.Query.GetPrimaryUnits;

public sealed record GetPrimaryUnitsResponse(Guid UnitId, string Name);

public sealed class GetPrimaryUnitsQueryHandler(IAppDbContext dbContext)
{
    public async Task<IEnumerable<GetPrimaryUnitsResponse>> Handle(CancellationToken cancellationToken)
    {
        return await dbContext.Units.AsNoTracking()
                                    .Where(unit => unit.PrimaryUnit == null)
                                    .Select(unit => new GetPrimaryUnitsResponse(unit.Id.Value, unit.Name))
                                    .ToListAsync(cancellationToken);
    }
}
