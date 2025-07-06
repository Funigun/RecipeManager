using System.Text.Json;

namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

public abstract class BaseIntegrationTest
{
    protected JsonSerializerOptions JsonOptions { get; private set; } = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
