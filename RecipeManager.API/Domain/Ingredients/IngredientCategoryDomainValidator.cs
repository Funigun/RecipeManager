using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class IngredientCategoryDomainValidator : IDomainModelValidator<IngredientCategory>
{
    public const int CategoryNameMaxLength = 100;

    public void Validate(IngredientCategory entity)
    {
        throw new NotImplementedException();
    }
}
