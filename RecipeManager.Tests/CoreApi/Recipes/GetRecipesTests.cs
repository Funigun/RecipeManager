using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class GetRecipesTests : BaseIntegrationTest
{
    public GetRecipesTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }
}
