using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Ingredients;

[Trait("Core.Api", "Ingredients")]
public sealed class CreateIngredientTests : BaseIntegrationTest
{
    public CreateIngredientTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }
}
