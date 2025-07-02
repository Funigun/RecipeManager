using System.Net.Http.Json;
using RecipeManager.Tests.IntegrationTests.CoreApi.TestFixtures;
using Xunit;

namespace RecipeManager.Tests.IntegrationTests.CoreApi.Units;

[Trait("Units", TestCategories.IntegrationTests)]
[Collection("Database collection")]
public sealed class CreateUnitTests : BaseIntegrationTest
{
    public CreateUnitTests(DbContainerFactory dockerServicesFactory) : base(dockerServicesFactory)
    {
    }

    [Fact]
    public async Task CreateUnit_ShouldReturn_CreatedUnit()
    {
        // Arrange
        var createUnitRequest = new
        {
            Name = "Test Unit",
            ShortName = "TU",
            Group = 1
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("/api/units", createUnitRequest, CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();
        Guid createdUnit = await response.Content.ReadFromJsonAsync<Guid>(CancellationToken.None);

        Assert.NotEqual(Guid.Empty, createdUnit);
    }
}
