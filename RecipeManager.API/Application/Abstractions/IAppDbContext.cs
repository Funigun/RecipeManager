using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RecipeManager.API.Domain.Units;

namespace RecipeManager.API.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Unit> Units { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);

    Task CommitTransaction(CancellationToken cancellationToken);

    Task RollbackTransaction(CancellationToken cancellationToken);
}
