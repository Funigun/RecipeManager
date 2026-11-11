using System.Net.Http.Headers;
using System.Text.Json;
using RecipeManager.Api.Features.RecipeCategories;
using RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategories;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.RecipeCategories;

[Trait("Core.Api", "RecipeCategories")]
public sealed class GetCategoriesTests : BaseIntegrationTest
{
    public GetCategoriesTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task GetRecipeCategories_ShouldReturn_NotAuthorized_WhenUserIsNotLoggedIn()
    {
        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(RecipeCategoriesGroup.Route, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeCategories_ShouldDisableChanges_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(RecipeCategoriesGroup.Route, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(content);

        GetRecipeCategoriesResponse? responseModel = JsonSerializer.Deserialize<GetRecipeCategoriesResponse>(content, JsonOptions);

        Assert.NotNull(responseModel);
        Assert.False(responseModel.Actions.CanAdd);
        Assert.NotEmpty(responseModel.Categories);
        Assert.All(responseModel.Categories, category =>
        {
            Assert.True(category.Actions.CanView);
            Assert.False(category.Actions.CanEdit);
            Assert.False(category.Actions.CanDelete);
        });
    }

    [Fact]
    public async Task GetRecipeCategories_ShouldEnableChanges_ForAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(RecipeCategoriesGroup.Route, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(content);

        GetRecipeCategoriesResponse? categories = JsonSerializer.Deserialize<GetRecipeCategoriesResponse>(content, JsonOptions);
        Assert.NotNull(categories);
        Assert.True(categories.Actions.CanAdd);
        Assert.NotEmpty(categories.Categories);
        Assert.All(categories.Categories, category =>
        {
            Assert.True(category.Actions.CanEdit);
            Assert.True(category.Actions.CanDelete);
        });
    }
}
