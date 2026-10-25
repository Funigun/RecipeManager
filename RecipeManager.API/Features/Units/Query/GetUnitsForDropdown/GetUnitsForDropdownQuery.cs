using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Persistance.Extensions;

namespace RecipeManager.Api.Features.Units.Query.GetUnitsForDropdown;

public sealed record GetUnitsForDropdownResponse(Guid Id, string Name);

public sealed class GetUnitsForDropdownQueryHandler(IAppDbContext dbContext)
{
    public async Task<IEnumerable<GetUnitsForDropdownResponse>> Handle(CancellationToken cancellationToken)
    {
        return await dbContext.Units.AsNoTracking()
                                    .DefaultOrder()
                                    .Select(unit => new GetUnitsForDropdownResponse(unit.Id.Value, unit.ShortName ?? unit.Name))
                                    .ToListAsync(cancellationToken);
    }
}
