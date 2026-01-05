using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Application.Database.Repositories;

public interface IUnitRepository : IBaseRepository<Unit, UnitId>
{
    Task<bool> AnyByNameAsync(string name, UnitId excludedId = null, CancellationToken cancellationToken = default);

    Task<bool> AnyByShortNameAsync(string name, UnitId excludedId = null, CancellationToken cancellationToken = default);
}
