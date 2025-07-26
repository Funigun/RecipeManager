using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Recipes.Validators;

public sealed class RecipeCategoryDomainValidator : IDomainModelValidator<RecipeCategory>
{
    public const int RecipeCategoryNameMaxLength = 100;

    public void Validate(RecipeCategory entity)
    {
        throw new NotImplementedException();
    }
}
