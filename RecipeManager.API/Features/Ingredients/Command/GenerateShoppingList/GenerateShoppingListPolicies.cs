using FluentValidation;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Features.Validation;

namespace RecipeManager.Api.Features.Ingredients.Command.GenerateShoppingList;

public sealed class GenerateShoppingListValidationPolicy : BaseValidator<GenerateShoppingListRequest>
{
    public GenerateShoppingListValidationPolicy(IAppDbContext dbContext)
    {
        RuleFor(request => request.Recipes)
            .NotEmpty()
            .WithMessage("At least one recipe ID must be provided.")
            .MustReferenceExistingRecipes(
                dbContext,
                recipes => recipes.Select(recipe => recipe.Id).Distinct(),
                "Some of the provided recipe IDs do not exist.");
    }
}
