using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class UpdateRecipeTests : BaseIntegrationTest
{
    public UpdateRecipeTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }
}
