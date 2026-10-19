using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes;

public static class UpdateRecipe
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed record RecipeIngredientDto(IngredientDto Ingredient, UnitDto Unit, double Amount);

    public sealed record IngredientDto(Guid Id, Guid? IngredientRecipe);

    public sealed record UnitDto(Guid Id);

    public sealed record RecipeAmountDto(double Amount, UnitDto Unit);

    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);

    public sealed record RecipeSectionDto(int SectionType, IEnumerable<RecipeStepDto> Steps);

    public sealed record RecipeStepDto(int Order, string Description, string? ImageUrl);

    public sealed record RecipeCategoryDto(Guid Id);

    public sealed record RecipeDto(string Title, string Description, string? ImageUrl, string? VideoUrl, RecipeAmountDto Amount, byte NumberOfServings, int Difficulty, NutritionalValueDto NutritionalValues,
                                   IEnumerable<RecipeIngredientDto> Ingredients, IEnumerable<RecipeSectionDto> Sections, IEnumerable<RecipeCategoryDto> Categories, Guid? IngredientId);

    public sealed class AuthorizationPolicy(IAppDbContext dbContext, ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            return await dbContext.Recipes.AnyAsync(recipe => recipe.Id == new RecipeId(request.Id) && recipe.CreatedBy == currentUser.Id);
        }
    }

    public sealed class Validator : AbstractValidator<RecipeDto>
    {
        public Validator(IAppDbContext dbContext, IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Title).ValidateRecipeTitle();
            RuleFor(x => x.Description).ValidateRecipeDescription();
            RuleFor(x => x.ImageUrl).ValidateRecipeImageUrl();
            RuleFor(x => x.VideoUrl).ValidateRecipeVideoUrl();
            RuleFor(x => x.Amount.Amount).ValidateRecipeAmount();
            RuleFor(x => x.Amount.Unit.Id).NotEmpty()
                .MustReferenceExistingUnit(unitOfWork, "Provided invalid UnitId for recipe amount");

            RuleFor(x => x.NumberOfServings).ValidateRecipeNumberOfServings();
            RuleFor(x => x.Difficulty)
                .Must(difficulty => difficulty.IsRecipeDifficulty())
                .WithMessage("Provided invalid recipe difficulty level");

            RuleFor(x => x.NutritionalValues.Calories).ValidateNutritionalCalories();
            RuleFor(x => x.NutritionalValues.Carbohydrates).ValidateNutritionalCarbohydrates();
            RuleFor(x => x.NutritionalValues.Fats).ValidateNutritionalFats();
            RuleFor(x => x.NutritionalValues.Proteins).ValidateNutritionalProteins();
            RuleFor(x => x.NutritionalValues.IngredientAmount).ValidateNutritionalAmount();

            RuleFor(x => x.Ingredients).SetValidator(new RecipeIngredientsValidator(dbContext, unitOfWork));
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
        public RecipeIngredientsValidator(IAppDbContext dbContext, IUnitOfWork unitOfWork)
        {
            RuleFor(ingredients => ingredients)
                .NotEmpty()
                .MustReferenceExistingIngredients(
                    dbContext,
                    ingredients => ingredients.Select(ingredient => ingredient.Ingredient.Id),
                    "Some of ingredients does not exist")
                .MustReferenceExistingUnits(
                    unitOfWork,
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

    [GroupEndpoint("Recipes")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPut<Request, RecipeDto>("/{recipeId}", Handler)
                     .WithName("UpdateRecipe")
                     .WithDescription("Updates existing recipe");
        }
    }

    public static async Task<IResult> Handler(Request recipeId, [FromBody] RecipeDto recipeDto, [FromServices] IAppDbContext dbContext, [FromServices] ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        RecipeId id = new(recipeId.Id);
        Recipe recipe = await dbContext.Recipes
                                       .Include(recipe => recipe.Ingredients)
                                       .Include(recipe => recipe.Sections)
                                            .ThenInclude(section => section.Steps)
                                       .Include(recipe => recipe.Categories)
                                       .FirstOrDefaultAsync(r => r.Id == id && r.CreatedBy == currentUser.Id, cancellationToken)
                     ?? throw new EntityNotFoundException<Recipe, RecipeId>(id);

        UpdateRecipeData(recipe, recipeDto);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }

    public static void UpdateRecipeData(Recipe recipe, RecipeDto recipeDto)
    {
        recipe.Title = recipeDto.Title;
        recipe.Description = recipeDto.Description;
        recipe.ImageURL = recipeDto.ImageUrl;
        recipe.VideoURL = recipeDto.VideoUrl;
        recipe.Amount = recipeDto.Amount.ToDomain();
        recipe.NumberOfServings = recipeDto.NumberOfServings;
        recipe.Difficulty = (RecipeDifficulty)recipeDto.Difficulty;
        recipe.NutritionalValue = recipeDto.NutritionalValues.ToNutritionalValue();
        recipe.IngredientId = recipeDto.IngredientId.HasValue ? new IngredientId(recipeDto.IngredientId.Value) : null;
        recipe.Ingredients = recipeDto.Ingredients.Select(i => i.ToDomain()).ToList();
        recipe.Sections = recipeDto.Sections.Select(s => s.ToDomain()).ToList();
        recipe.UpdateCategories(recipeDto.Categories.Select(category => new RecipeCategoryId(category.Id)).ToList());
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
