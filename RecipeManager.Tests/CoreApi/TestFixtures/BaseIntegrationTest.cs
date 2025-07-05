namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

public class BaseIntegrationTest : IAsyncLifetime
{
    protected DbContainerFactory DockerServicesFactory { get; private set; }

    protected WebApiFactory WebApiFactory { get; private set; }

    protected HttpClient HttpClient { get; private set; }

    protected BaseIntegrationTest(DbContainerFactory dockerServicesFactory)
    {
        DockerServicesFactory = dockerServicesFactory;
        WebApiFactory = new WebApiFactory(DockerServicesFactory.GetConnectionString());
        HttpClient = WebApiFactory.CreateClient();
    }

    public async ValueTask InitializeAsync()
    {

    }

    public async ValueTask DisposeAsync()
    {
        HttpClient?.Dispose();
        WebApiFactory?.Dispose();

        await ValueTask.CompletedTask;
    }
}
