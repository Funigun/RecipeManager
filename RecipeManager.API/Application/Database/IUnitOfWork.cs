using Microsoft.EntityFrameworkCore.Storage;
using RecipeManager.Api.Application.Database.Repositories;

namespace RecipeManager.Api.Application.Database;

public interface IUnitOfWork
{
    IUnitRepository Units { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
}
