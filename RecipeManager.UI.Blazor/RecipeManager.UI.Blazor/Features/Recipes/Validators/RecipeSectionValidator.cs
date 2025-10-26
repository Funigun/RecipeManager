using FluentValidation;
using RecipeManager.Shared.Contracts.Recipes;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Recipes.Models;

namespace RecipeManager.UI.Blazor.Features.Recipes.Validators;

public sealed class RecipeSectionValidator : BaseAbstractValidator<RecipeSectionModel>
{
    public RecipeSectionValidator()
    {
        When(section => section.IsRequired, () =>
        {
            RuleFor(s => s.Steps)
                   .NotEmpty()
                   .WithMessage("Section requires at least one step");

            RuleForEach(s => s.Steps)
                   .ChildRules(step =>
                   {
                       step.RuleFor(s => s.Order).GreaterThan(0);
                       step.RuleFor(s => s.Description).SetValidator(new RecipeStepDescriptionValidator());
                       step.RuleFor(s => s.ImageUrl).SetValidator(new RecipeImageUrlValidator());
                   });
        });
    }
}
