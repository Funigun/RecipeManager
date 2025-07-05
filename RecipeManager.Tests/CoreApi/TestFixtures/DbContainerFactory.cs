using DotNet.Testcontainers.Builders;
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

    public DbContainerFactory()
    {

    }

    public string GetConnectionString() => _sqlContainer.GetConnectionString();

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await _sqlContainer.StartAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _sqlContainer.StopAsync();
    }

    public async void Dispose()
    {
        await _sqlContainer.DisposeAsync();
    }
}
