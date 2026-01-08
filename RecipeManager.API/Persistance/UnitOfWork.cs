using Microsoft.EntityFrameworkCore.Storage;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Application.Database.Repositories;

namespace RecipeManager.Api.Persistance;

public class UnitOfWork(AppDbContext dbContext, IUnitRepository unitRepository) : IUnitOfWork, IDisposable
{
    private bool disposed;

    public IUnitRepository Units { get; } = unitRepository;

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.BeginTransaction(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.CommitTransaction(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            dbContext.Dispose();
        }

        disposed = true;
    }
}
