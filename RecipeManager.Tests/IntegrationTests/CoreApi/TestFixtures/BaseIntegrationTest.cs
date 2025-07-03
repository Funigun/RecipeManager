using Microsoft.EntityFrameworkCore;
using RecipeManager.API.Persistance;
using Respawn;

namespace RecipeManager.Tests.IntegrationTests.CoreApi.TestFixtures;

public class BaseIntegrationTest : IAsyncLifetime
{
    private static readonly Lock _lock = new();
    private static bool _databaseInitialized;
    private static bool _databaseSeeded;
    private static Respawner _respawner = default!;

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
                DbContext = new(options);
                DbContext.Database.Migrate();

                _databaseInitialized = true;
            }
            else
            {
                DbContext = new(options);
            }
        }

        WebApiFactory = new WebApiFactory(DockerServicesFactory.GetConnectionString());
        HttpClient = WebApiFactory.CreateClient();
    }

    public async ValueTask InitializeAsync()
    {
        if (!_databaseSeeded)
        {
            _respawner = await Respawner.CreateAsync(DockerServicesFactory.GetConnectionString(),
                         new RespawnerOptions
                         {
                             DbAdapter = DbAdapter.SqlServer,
                             SchemasToInclude = new[] { "dbo" },
                         });

            _databaseSeeded = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _respawner.ResetAsync(DockerServicesFactory.GetConnectionString());
        _databaseSeeded = false;
    }
}
