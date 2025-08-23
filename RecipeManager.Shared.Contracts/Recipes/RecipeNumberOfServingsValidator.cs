using FluentValidation;

namespace RecipeManager.Shared.Contracts.Recipes;

public class RecipeNumberOfServingsValidator : AbstractValidator<byte>
{
    public RecipeNumberOfServingsValidator()
    {
        RuleFor(x => x)
            .InclusiveBetween((byte)1, (byte)255);
    }
}
