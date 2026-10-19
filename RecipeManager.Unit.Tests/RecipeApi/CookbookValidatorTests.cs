using RecipeManager.Api.Features.Cookbooks;

namespace RecipeManager.Unit.Tests.RecipeApi;

public sealed class CookbookValidatorTests
{
    [Fact]
    public void Validator_ReportsInvalidNestedCategoryNameAtItsPath()
    {
        CreateCookbook.Validator validator = new();
        CreateCookbook.Request request = new(
            "My Cookbook",
            "A cookbook description",
            null,
            [new("Main", [], [new(string.Empty, [], [])])]);

        FluentValidation.Results.ValidationResult result = validator.Validate(request);

        Assert.Contains(result.Errors, failure =>
            failure.PropertyName == "Categories[0].Subcategories[0].Name" &&
            failure.ErrorMessage == "Category name must not be empty.");
    }
}
