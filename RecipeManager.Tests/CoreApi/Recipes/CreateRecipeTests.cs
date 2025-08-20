using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Recipes;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class CreateRecipeTests : BaseIntegrationTest
{
    public CreateRecipeTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task CreateRecipe_ShouldReturnId_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        CreateRecipe.Request request = await CreateFakeRecipe();
        StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/recipes", content, CancellationToken.None);
        string responseBody = await response.Content.ReadAsStringAsync();
        CreateRecipe.Response? recipeId = JsonSerializer.Deserialize<CreateRecipe.Response>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(recipeId);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/recipes/{recipeId.Id}", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private async Task<CreateRecipe.Request> CreateFakeRecipe()
    {
        UnitId unitId = (await DbContext.Units.AsNoTracking().FirstAsync(CancellationToken.None)).Id;
        IngredientId ingredientId = (await DbContext.Ingredients.AsNoTracking().FirstAsync(CancellationToken.None)).Id;

        CreateRecipe.RecipeAmountDto amount = new(10d, unitId.Value);
        CreateRecipe.RecipeIngredientDto ingredientDto = new(ingredientId.Value, unitId.Value, 10d);
        CreateRecipe.RecipeSectionDto ingredientsPreparation = new((int)RecipeSectionType.IngredientsPreparation, [new CreateRecipe.RecipeStepDto(1, "Test", null)]);
        CreateRecipe.RecipeSectionDto cooking = new((int)RecipeSectionType.Cooking, [new CreateRecipe.RecipeStepDto(2, "Test 2", null)]);

        return new CreateRecipe.Request
        (
            "New fake recipe",
            "This is a test recipe",
            null,
            null,
            amount,
            1,
            (int)RecipeDifficulty.Easy,
            [ingredientDto],
            [ingredientsPreparation, cooking],
            [],
            null
        );
    }
}
