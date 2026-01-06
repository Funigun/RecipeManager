using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Database.Repositories;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Persistance.Repositories;

public class UnitRepository(AppDbContext dbContext, ICurrentUser currentUser)
           : BaseRepository<Unit, UnitId>(dbContext, currentUser), IUnitRepository
{
    public async Task<bool> AnyByNameAsync(string name, UnitId excludedId = null, CancellationToken cancellationToken = default)
    {
        return await dbContext.Units.AnyAsync(unit => unit.Name == name && unit.Id != excludedId, cancellationToken);
    }

    public async Task<bool> AnyByShortNameAsync(string name, UnitId excludedId = null, CancellationToken cancellationToken = default)
    {
        return await dbContext.Units.AnyAsync(unit => unit.ShortName == name && unit.Id != excludedId, cancellationToken);
    }

    public async Task<IEnumerable<Unit>> GetPrimaryUnitsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Units.AsNoTracking()
                              .Where(unit => unit.PrimaryUnit == null)
                              .ToListAsync(cancellationToken);
    }
}
