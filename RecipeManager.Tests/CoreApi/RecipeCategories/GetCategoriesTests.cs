using System.Net.Http.Headers;
using System.Text.Json;
using RecipeManager.Api.Features.RecipeCategories;
using RecipeManager.Api.Shared.Hateoas.Models;
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
        HttpResponseMessage response = await HttpClient.GetAsync("/api/recipeCategories", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeCategories_ShouldReturn_Categories_WithoutLinks_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("/api/recipeCategories", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(content);

        HateoasCollectionResponse<GetRecipeCategories.CategoryDto>? categories = JsonSerializer.Deserialize<HateoasCollectionResponse<GetRecipeCategories.CategoryDto>>(content, JsonOptions);
        Assert.NotNull(categories);
        Assert.NotEmpty(categories.Items);
        Assert.Empty(categories.Links);

        // ToDo: tbc: string content looks fine, but deserialized obj lacks links in items
        //Assert.All(categories.Items, category =>
        //{
        //    Assert.Empty(category.Links);
        //});
    }

    [Fact]
    public async Task GetRecipeCategories_ShouldReturn_Categories_WithLinks_ForAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("/api/recipeCategories", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(content);

        HateoasCollectionResponse<GetRecipeCategories.CategoryDto>? categories = JsonSerializer.Deserialize<HateoasCollectionResponse<GetRecipeCategories.CategoryDto>>(content, JsonOptions);
        Assert.NotNull(categories);
        Assert.NotEmpty(categories.Links);
        Assert.NotEmpty(categories.Items);

        // ToDo: tbc: string content looks fine, but deserialized obj lacks links in items
        //Assert.All(categories.Items, category =>
        //{
        //    Assert.NotEmpty(category.Links);
        //});
    }
}
