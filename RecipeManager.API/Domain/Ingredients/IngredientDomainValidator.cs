using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class IngredientDomainValidator : IDomainModelValidator<Ingredient>
{
    public const int IngredientNameMaxLength = 100;

    public void Validate(Ingredient entity)
    {
        throw new NotImplementedException();
    }
}
