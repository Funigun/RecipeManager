using FluentValidation;
using RecipeManager.Shared.Contracts.Recipes;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Recipes.Models;

namespace RecipeManager.UI.Blazor.Features.Recipes.Validators;

public sealed class RecipeModelValidator : BaseAbstractValidator<HateoasResponse<RecipeModel>>
{
    public RecipeModelValidator()
    {
        RuleFor(x => x.Item.Title).SetValidator(new RecipeTitleValidator());
        RuleFor(x => x.Item.Description).SetValidator(new RecipeDescriptionValidator());
        RuleFor(x => x.Item.ImageUrl).SetValidator(new RecipeImageUrlValidator());
        RuleFor(x => x.Item.VideoUrl).SetValidator(new RecipeVideoUrlValidator());
        RuleFor(x => x.Item.Amount.Amount).SetValidator(new RecipeAmountValidator());
        RuleFor(x => x.Item.Amount.Unit.Id).NotEmpty()
                                                                .WithMessage("Units for recipe amount must be specified");
        RuleFor(x => x.Item.NumberOfServings).SetValidator(new RecipeNumberOfServingsValidator());
    }
}
