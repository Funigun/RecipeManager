using Microsoft.EntityFrameworkCore;
using RecipeManager.API.Persistance;
using RecipeManager.Integration.Factory.Tests.Common.Users;

namespace RecipeManager.Integration.Factory.Tests.CoreApi.TestFixtures;

public class BaseIntegrationTest : IAsyncLifetime
{
    private static readonly Lock _lock = new();
    private static bool _databaseInitialized;
    private static bool _databaseSeeded;

    protected DbContainerFactory DockerServicesFactory { get; private set; }

    protected AppDbContext DbContext { get; private set; }

    protected WebApiFactory WebApiFactory { get; private set; }

    protected HttpClient HttpClient { get; private set; }

    protected BaseIntegrationTest(DbContainerFactory dockerServicesFactory)
    {
        DockerServicesFactory = dockerServicesFactory;

        lock (_lock)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                                                          .UseSqlServer(DockerServicesFactory.GetConnectionString())
                                                          .Options;
            if (!_databaseInitialized)
            {
                DbContext = new(options, UserMockFactory.CreateMockedAdmin());
                DbContext.Database.Migrate();

                _databaseInitialized = true;
            }
            else
            {
                DbContext = new(options, UserMockFactory.CreateMockedAdmin());
            }
        }

        WebApiFactory = new WebApiFactory(DockerServicesFactory.GetConnectionString());
        HttpClient = WebApiFactory.CreateClient();
    }

    public async ValueTask InitializeAsync()
    {
        if (!_databaseSeeded)
        {
            await DbContext.SeedAsync();

            _databaseSeeded = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await ValueTask.CompletedTask;
    }
}
