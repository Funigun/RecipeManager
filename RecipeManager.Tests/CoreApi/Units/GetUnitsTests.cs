using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;
using RecipeManager.Api.Features.Units.Query.GetAllUnits;
using RecipeManager.Api.Features.Units.Query.GetUnitById;

namespace RecipeManager.Integration.Tests.CoreApi.Units;

[Trait("Core.Api", "Units")]
public sealed class GetUnitsTests : BaseIntegrationTest
{
    public GetUnitsTests(WebApiFactory apiFactory) : base(apiFactory)
    {
    }

    [Fact]
    public async Task GetUnits_ShouldReturn_NotAuthorized_WhenUserIsNotLoggedIn()
    {
        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(UnitsGroup.Route, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUnits_ShouldReturn_DisabledActions_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(UnitsGroup.Route, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(content);

        GetAllUnitsCollectionResponse? units = JsonSerializer.Deserialize<GetAllUnitsCollectionResponse>(content, JsonOptions);
        Assert.NotNull(units);
        Assert.NotEmpty(units.Units);
        Assert.False(units.Actions.CanAdd);
        Assert.All(units.Units, unit =>
        {
            Assert.True(unit.Actions.CanView);
            Assert.False(unit.Actions.CanEdit);
            Assert.False(unit.Actions.CanDelete);
        });
    }

    [Fact]
    public async Task GetAllUnits_ShouldReturn_EnabledActions_ForAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(UnitsGroup.Route, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(content);

        GetAllUnitsCollectionResponse? units = JsonSerializer.Deserialize<GetAllUnitsCollectionResponse>(content, JsonOptions);
        Assert.NotNull(units);
        Assert.NotEmpty(units.Units);
        Assert.True(units.Actions.CanAdd);
        Assert.All(units.Units, unit =>
        {
            Assert.True(unit.Actions.CanView);
            Assert.True(unit.Actions.CanEdit);
            Assert.True(unit.Actions.CanDelete);
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetUnitById_ShouldReturnActions_ForCurrentUser(bool isAdmin)
    {
        // Arrange
        ICurrentUser user = isAdmin ? UserMockFactory.CreateMockedAdmin() : UserMockFactory.CreateMockedUser();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(user));
        Guid unitId = (await DbContext.Units.AsNoTracking().FirstAsync(TestContext.Current.CancellationToken)).Id.Value;

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(UnitsGroup.UnitById(unitId), TestContext.Current.CancellationToken);
        GetUnitByIdResponse? unit = await response.Content.ReadFromJsonAsync<GetUnitByIdResponse>(JsonOptions, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(unit);
        Assert.True(unit.Actions.CanView);
        Assert.Equal(isAdmin, unit.Actions.CanEdit);
        Assert.Equal(isAdmin, unit.Actions.CanDelete);
    }

    [Fact]
    public async Task GetUnitById_ShouldReturn_NotFound_ForNonExistingUnit()
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        HttpResponseMessage response = await HttpClient.GetAsync(UnitsGroup.UnitById(Guid.CreateVersion7()), TestContext.Current.CancellationToken);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
