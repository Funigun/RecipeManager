using DotNet.Testcontainers.Builders;
using Testcontainers.MsSql;

namespace RecipeManager.Tests.IntegrationTests.CoreApi.TestFixtures;

public sealed class DbContainerFactory : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                                                                       .WithPassword("Str0ng_P@ssw0rd4Tests")
                                                                       .WithPortBinding(1433)
                                                                       .WithEnvironment("ACCEPT_EULA", "Y")
                                                                       .WithName("MealsManagerTestDb")
                                                                       .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                                                                       .Build();

    public DbContainerFactory()
    {

    }

    public string GetConnectionString() => _sqlContainer.GetConnectionString();

    ValueTask IAsyncLifetime.InitializeAsync()
    {
        _sqlContainer.StopAsync(CancellationToken.None);

        return _sqlContainer.DisposeAsync();
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        _sqlContainer.StartAsync(CancellationToken.None);

        return ValueTask.CompletedTask;
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DbContainerFactory>
{
    // No code needed here—just the attribute and interface
}
