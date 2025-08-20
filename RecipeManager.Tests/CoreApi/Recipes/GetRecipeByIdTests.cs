using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class GetRecipeByIdTests : BaseIntegrationTest
{
    public GetRecipeByIdTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }
}
