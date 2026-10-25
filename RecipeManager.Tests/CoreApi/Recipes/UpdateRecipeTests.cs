using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Recipes;
using RecipeManager.Api.Features.Recipes.Command.UpdateRecipe;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class UpdateRecipeTests : BaseIntegrationTest
{
    public UpdateRecipeTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task UpdateRecipe_ShouldReturnNoContent_ForValidUpdate()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        Recipe recipe = await DbContext.Recipes.Include(r => r.Ingredients)
                                               .Include(r => r.Categories)
                                               .Include(r => r.Sections)
                                                .ThenInclude(s => s.Steps)
                                               .AsSplitQuery()
                                               .AsNoTracking()
                                               .FirstAsync(recipe => recipe.Title == "To Update", TestContext.Current.CancellationToken);

        UpdateRecipeRequest recipeDto = new
        (
            recipe.Title,
            "Updated description",
            null,
            null,
            new UpdateRecipeAmountDto(500d, new(recipe.Amount.UnitId.Value)),
            5,
            (int)recipe.Difficulty,
            new(1, 2, 1, 1, 1, recipe.NutritionalValue.IngredientUnit),
            recipe.Ingredients.Select(ingredient => new UpdateRecipeIngredientDto(new(ingredient.IngredientId, null), new(ingredient.UnitId), ingredient.Amount)).ToList(),
            recipe.Sections.Select(section => new UpdateRecipeSectionDto((int)section.Type, section.Steps.Select(step => new UpdateRecipeStepDto(step.Order, step.Description, null)).ToList())).ToList(),
            [],
            null
        );

        using StringContent content = new(JsonSerializer.Serialize(recipeDto), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync(RecipesGroup.RecipeByRecipeId(recipe.Id.Value), content, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
