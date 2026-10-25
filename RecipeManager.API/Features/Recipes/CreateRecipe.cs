using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes;

public static class CreateRecipe
{
    public sealed record RecipeIngredientDto(IngredientDto Ingredient, UnitDto Unit, double Amount);

    public sealed record IngredientDto(Guid Id, Guid? IngredientRecipe);

    public sealed record UnitDto(Guid Id);

    public sealed record RecipeAmountDto(double Amount, UnitDto Unit);

    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);

    public sealed record RecipeSectionDto(int SectionType, IEnumerable<RecipeStepDto> Steps);

    public sealed record RecipeStepDto(int Order, string Description, string? ImageUrl);

    public sealed record RecipeCategory(Guid Id);

    public sealed record Request(string Title, string Description, string? ImageUrl, string? VideoUrl, RecipeAmountDto Amount, byte NumberOfServings, int Difficulty, NutritionalValueDto NutritionalValues,
                                 IEnumerable<RecipeIngredientDto> Ingredients, IEnumerable<RecipeSectionDto> Sections, IEnumerable<RecipeCategory> Categories, Guid? IngredientId);

    public sealed record Response(Guid Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Title).ValidateRecipeTitle();
            RuleFor(x => x.Description).ValidateRecipeDescription();
            RuleFor(x => x.ImageUrl).ValidateRecipeImageUrl();
            RuleFor(x => x.VideoUrl).ValidateRecipeVideoUrl();
            RuleFor(x => x.Amount.Amount).ValidateRecipeAmount();
            RuleFor(x => x.Amount.Unit.Id).NotEmpty()
                .MustReferenceExistingUnit(dbContext, "Provided invalid UnitId for recipe amount");

            RuleFor(x => x.NumberOfServings).ValidateRecipeNumberOfServings();
            RuleFor(x => x.Difficulty)
                .Must(difficulty => difficulty.IsRecipeDifficulty())
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
                .Must((request, ingredientId) =>
                {
                    return ingredientId is null || !request.Ingredients.Any(ingredient => ingredient.Ingredient.Id == ingredientId.Value);
                })
                .WithMessage("List of ingredients can not contain an ingredient for which this recipe is designed");
        }
    }

    public sealed class RecipeIngredientsValidator : AbstractValidator<IEnumerable<RecipeIngredientDto>>
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

            RuleForEach(x => x)
                .ChildRules(ingredient =>
                {
                    ingredient.RuleFor(i => i.Amount).ValidateRecipeAmount();
                });
        }
    }

    public sealed class RecipeSectionsValidator : AbstractValidator<IEnumerable<RecipeSectionDto>>
    {
        public RecipeSectionsValidator()
        {
            RuleFor(sections => sections)
                .Must(sections =>
                {
                    IEnumerable<int> sectionTypes = sections.Select(s => s.SectionType);
                    return sectionTypes.Distinct().Count() == sectionTypes.Count();
                })
                .WithMessage("Recipe sections must have unique types")
                .Must(sections =>
                {
                    IEnumerable<int> sectionTypes = sections.Select(s => s.SectionType);
                    return sectionTypes.Any(type => type == (int)RecipeSectionType.IngredientsPreparation) &&
                           sectionTypes.Any(type => type == (int)RecipeSectionType.Cooking);
                })
                .WithMessage("Recipe requires at least Ingredients Preparation and Cooking sections");

            RuleForEach(x => x)
                .ChildRules(section =>
                {
                    section.RuleFor(s => s.SectionType)
                           .Must(type => type.IsRecipeSectionType())
                           .WithMessage("Provided invalid recipe section type");

                    section.RuleFor(s => s.Steps)
                           .NotEmpty()
                                .WithMessage("Recipe section must have at least one step")
                           .Must(steps =>
                           {
                               IEnumerable<int> stepOrders = steps.Select(s => s.Order);
                               return stepOrders.Distinct().Count() == stepOrders.Count();
                           }).WithMessage("Recipe section must have steps with unique Order values")
                           .Must(steps =>
                           {
                               List<int>? orders = steps.Select(s => s.Order).OrderBy(o => o).ToList();
                               return orders.SequenceEqual(Enumerable.Range(1, orders.Count));
                           }).WithMessage("Step orders must be consecutive starting from 1.");

                    section.RuleForEach(s => s.Steps)
                           .ChildRules(step =>
                           {
                               step.RuleFor(s => s.Order).GreaterThan(0);
                               step.RuleFor(s => s.Description).ValidateRecipeStepDescription();
                               step.RuleFor(s => s.ImageUrl).ValidateRecipeImageUrl();
                           });
                });
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardValidatedPost<Request, Response>(RecipesGroup.Route, Handler)
                     .WithName("CreateRecipe")
                     .WithDescription("Creates a new recipe")
                     .WithTags(RecipesGroup.Tag)
                     .RequireAuthorization("RecipeManagerPolicy")
                     .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }

    public static async Task<IResult> Handler([FromBody] Request request, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Recipe recipe = request.ToDomain();

        dbContext.Recipes.Add(recipe);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created(RecipesGroup.RecipeById(recipe.Id.Value), new Response(recipe.Id.Value));
    }

    public static Recipe ToDomain(this Request request)
    {
        return Recipe.Create(
            request.Title,
            request.Amount.ToDomain(),
            request.NumberOfServings,
            (RecipeDifficulty)request.Difficulty,
            request.NutritionalValues.ToNutritionalValue(),
            request.Ingredients.Select(ToDomain),
            request.Sections.Select(ToDomain),
            request.Categories.Select(c => new RecipeCategoryId(c.Id)),
            request.IngredientId is Guid id ? new IngredientId(id) : null!,
            request.Description,
            request.ImageUrl,
            request.VideoUrl
        );
    }

    private static RecipeAmount ToDomain(this RecipeAmountDto dto)
    {
        return new(dto.Amount, new UnitId(dto.Unit.Id));
    }

    private static RecipeIngredient ToDomain(this RecipeIngredientDto dto)
    {
        return RecipeIngredient.Create
        (
            new IngredientId(dto.Ingredient.Id),
            new UnitId(dto.Unit.Id),
            dto.Amount
        );
    }

    private static RecipeSection ToDomain(this RecipeSectionDto dto)
    {
        List<RecipeStep> steps = dto.Steps.Select(s => s.ToDomain()).ToList();

        return RecipeSection.Create(dto.SectionType, steps);
    }

    private static RecipeStep ToDomain(this RecipeStepDto dto)
    {
        return RecipeStep.Create
        (
            dto.Order,
            dto.Description,
            dto.ImageUrl
        );
    }

    private static NutritionalValue ToNutritionalValue(this NutritionalValueDto dto)
    {
        return new NutritionalValue
        {
            Calories = dto.Calories,
            Proteins = dto.Proteins,
            Fats = dto.Fats,
            Carbohydrates = dto.Carbohydrates,
            IngredientAmount = dto.IngredientAmount,
            IngredientUnit = new UnitId(dto.IngredientUnitId)
        };
    }
}
