using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Persistance;
using RecipeManager.Integration.Tests.Common.Users;

namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected JsonSerializerOptions JsonOptions { get; private set; } = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected HttpClient HttpClient { get; private set; }

    protected AppDbContext DbContext { get; private set; }

    protected BaseIntegrationTest(WebApiFactory webApiFactory)
    {
        HttpClient = webApiFactory.CreateClient();

        DbContext = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(webApiFactory.ConnectionString)
                .Options,
            UserMockFactory.CreateMockedAdmin()
        );
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
