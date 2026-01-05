using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Database.Repositories;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Persistance.Repositories;

public class UnitRepository(AppDbContext dbContext, ICurrentUser currentUser)
           : BaseRepository<Unit, UnitId>(dbContext, currentUser), IUnitRepository
{
    public Task<bool> AnyByNameAsync(string name, UnitId excludedId = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyByShortNameAsync(string name, UnitId excludedId = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Unit>> GetPrimaryUnitsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Units.AsNoTracking()
                              .Where(unit => unit.PrimaryUnit == null)
                              .ToListAsync(cancellationToken);
    }
}
