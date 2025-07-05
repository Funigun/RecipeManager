using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Persistance;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(DbContainerFactory))]

namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

public sealed class DbContainerFactory : IAsyncLifetime, IDisposable
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                                                                      .WithPassword("Str0ng_P@ssw0rd4Tests")
                                                                      .WithPortBinding(1433)
                                                                      .WithEnvironment("ACCEPT_EULA", "Y")
                                                                      .WithName("MealsManagerTestDb")
                                                                      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                                                                      .Build();

    private AppDbContext _dbContext = default!;

    public DbContainerFactory()
    {
    }

    public string GetConnectionString() => _sqlContainer.GetConnectionString();

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                                                 .UseSqlServer(_sqlContainer.GetConnectionString())
                                                 .Options;

        _dbContext = new(options, UserMockFactory.CreateMockedAdmin());

        await _dbContext.Database.MigrateAsync();
        await _dbContext.SeedAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _sqlContainer.StopAsync();
    }

    public void Dispose()
    {
        _sqlContainer.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
