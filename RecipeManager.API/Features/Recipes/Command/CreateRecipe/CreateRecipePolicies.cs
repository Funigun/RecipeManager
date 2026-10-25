using FluentValidation;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Features.Validation;

namespace RecipeManager.Api.Features.Recipes.Command.CreateRecipe;

public sealed class CreateRecipeValidationPolicy : BaseValidator<CreateRecipeRequest>
{
    public CreateRecipeValidationPolicy(IAppDbContext dbContext)
    {
        RuleFor(x => x.Title).ValidateRecipeTitle();
        RuleFor(x => x.Description).ValidateRecipeDescription();
        RuleFor(x => x.ImageUrl).ValidateRecipeImageUrl();
        RuleFor(x => x.VideoUrl).ValidateRecipeVideoUrl();
        RuleFor(x => x.Amount.Amount).ValidateRecipeAmount();
        RuleFor(x => x.Amount.Unit.Id).NotEmpty()
            .MustReferenceExistingUnit(dbContext, "Provided invalid UnitId for recipe amount");
        RuleFor(x => x.NumberOfServings).ValidateRecipeNumberOfServings();
        RuleFor(x => x.Difficulty).Must(value => value.IsRecipeDifficulty())
            .WithMessage("Provided invalid recipe difficulty level");
        RuleFor(x => x.NutritionalValues.Calories).ValidateNutritionalCalories();
        RuleFor(x => x.NutritionalValues.Carbohydrates).ValidateNutritionalCarbohydrates();
        RuleFor(x => x.NutritionalValues.Fats).ValidateNutritionalFats();
        RuleFor(x => x.NutritionalValues.Proteins).ValidateNutritionalProteins();
        RuleFor(x => x.NutritionalValues.IngredientAmount).ValidateNutritionalAmount();
        RuleFor(x => x.Ingredients).SetValidator(new RecipeIngredientsValidator(dbContext));
        RuleFor(x => x.Sections).SetValidator(new RecipeSectionsValidator());
        RuleFor(x => x.Categories)
            .MustHaveUniqueIds(category => category.Id, "Duplicate categories are not allowed.")
            .MustReferenceExistingRecipeCategories(
                dbContext,
                categories => categories.Select(category => category.Id),
                "Some of recipe categories does not exist");
        RuleFor(x => x.IngredientId)
            .MustReferenceExistingIngredient(dbContext, "Provided invalid IngredientId for recipe")
            .Must((request, ingredientId) => ingredientId is null ||
                !request.Ingredients.Any(ingredient => ingredient.Ingredient.Id == ingredientId.Value))
            .WithMessage("List of ingredients can not contain an ingredient for which this recipe is designed");
    }
}

internal sealed class RecipeIngredientsValidator : AbstractValidator<IEnumerable<CreateRecipeIngredientDto>>
{
    public RecipeIngredientsValidator(IAppDbContext dbContext)
    {
        RuleFor(ingredients => ingredients)
            .NotEmpty()
            .MustReferenceExistingIngredients(
                dbContext,
                ingredients => ingredients.Select(ingredient => ingredient.Ingredient.Id),
                "Some of ingredients does not exist")
            .MustReferenceExistingUnits(
                dbContext,
                ingredient => ingredient.Unit.Id,
                "Some of units does not exist")
            .MustHaveUniqueIds(ingredient => ingredient.Ingredient.Id, "Duplicate ingredients are not allowed");

        RuleForEach(ingredients => ingredients).ChildRules(ingredient =>
            ingredient.RuleFor(item => item.Amount).ValidateRecipeAmount());
    }
}

internal sealed class RecipeSectionsValidator : AbstractValidator<IEnumerable<CreateRecipeSectionDto>>
{
    public RecipeSectionsValidator()
    {
        RuleFor(sections => sections)
            .Must(sections =>
            {
                var types = sections.Select(section => section.SectionType);
                return types.Distinct().Count() == types.Count();
            })
            .WithMessage("Recipe sections must have unique types")
            .Must(sections =>
            {
                var types = sections.Select(section => section.SectionType);
                return types.Any(type => type == (int)RecipeSectionType.IngredientsPreparation) &&
                       types.Any(type => type == (int)RecipeSectionType.Cooking);
            })
            .WithMessage("Recipe requires at least Ingredients Preparation and Cooking sections");

        RuleForEach(sections => sections).ChildRules(section =>
        {
            section.RuleFor(item => item.SectionType).Must(type => type.IsRecipeSectionType())
                .WithMessage("Provided invalid recipe section type");
            section.RuleFor(item => item.Steps)
                .NotEmpty().WithMessage("Recipe section must have at least one step")
                .Must(steps => steps.Select(step => step.Order).Distinct().Count() == steps.Count())
                .WithMessage("Recipe section must have steps with unique Order values")
                .Must(steps =>
                {
                    var orders = steps.Select(step => step.Order).OrderBy(order => order).ToList();
                    return orders.SequenceEqual(Enumerable.Range(1, orders.Count));
                })
                .WithMessage("Step orders must be consecutive starting from 1.");
            section.RuleForEach(item => item.Steps).ChildRules(step =>
            {
                step.RuleFor(item => item.Order).GreaterThan(0);
                step.RuleFor(item => item.Description).ValidateRecipeStepDescription();
                step.RuleFor(item => item.ImageUrl).ValidateRecipeImageUrl();
            });
        });
    }
}
