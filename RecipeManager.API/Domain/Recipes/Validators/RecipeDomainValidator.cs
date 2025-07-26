using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Recipes.Validators;

public sealed class RecipeDomainValidator : IDomainModelValidator<Recipe>
{
    public const int RecipeTitleMaxLength = 150;

    public const int RecipeDescriptionMaxLength = 1500;

    public const int MinimumIngredientsCount = 1;

    public const int MinimumNumberOfServings = 1;

    public const int RecipeStepDescriptionMaxLength = 500;

    public void Validate(Recipe entity)
    {
        throw new NotImplementedException();
    }
}
