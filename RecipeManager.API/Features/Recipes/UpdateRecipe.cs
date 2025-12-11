using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Ingredients;
using RecipeManager.Shared.Contracts.Recipes;

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
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Title).SetValidator(new RecipeTitleValidator());
            RuleFor(x => x.Description).SetValidator(new RecipeDescriptionValidator());
            RuleFor(x => x.ImageUrl).SetValidator(new RecipeImageUrlValidator());
            RuleFor(x => x.VideoUrl).SetValidator(new RecipeVideoUrlValidator());
            RuleFor(x => x.Amount.Amount).SetValidator(new RecipeAmountValidator());
            RuleFor(x => x.Amount.Unit.Id).NotEmpty()
                .MustAsync(async (unitId, cancellationToken) =>
                {
                    return await dbContext.Units.AsNoTracking()
                                                .AnyAsync(unit => unit.Id == new UnitId(unitId), cancellationToken);
                })
                .WithMessage("Provided invalid UnitId for recipe amount");

            RuleFor(x => x.NumberOfServings).SetValidator(new RecipeNumberOfServingsValidator());
            RuleFor(x => x.Difficulty)
                .Must(difficulty => difficulty.IsRecipeDifficulty())
                .WithMessage("Provided invalid recipe difficulty level");

            RuleFor(x => x.NutritionalValues.Calories).SetValidator(new IngredientNutritionalCaloriesValueValidator());
            RuleFor(x => x.NutritionalValues.Carbohydrates).SetValidator(new IngredientNutritionalCarbohydratesValueValidator());
            RuleFor(x => x.NutritionalValues.Fats).SetValidator(new IngredientNutritionalFatsValueValidator());
            RuleFor(x => x.NutritionalValues.Proteins).SetValidator(new IngredientNutritionalProteinsValueValidator());
            RuleFor(x => x.NutritionalValues.IngredientAmount).SetValidator(new IngredientNutritionalAmountValidator());

            RuleFor(x => x.Ingredients).SetValidator(new RecipeIngredientsValidator(dbContext));
            RuleFor(x => x.Sections).SetValidator(new RecipeSectionsValidator());

            RuleFor(x => x.Categories)
                .Must(ids => ids.Distinct().Count() == ids.Count())
                    .WithMessage("Duplicate categories are not allowed.")
                .MustAsync(async (categoryIds, cancellationToken) =>
                {
                    if (!categoryIds.Any())
                    {
                        return true;
                    }

                    IEnumerable<RecipeCategoryId> recipeCategoryIds = categoryIds.Select(c => new RecipeCategoryId(c.Id));

                    return await dbContext.RecipeCategories.AsNoTracking()
                                                            .Where(category => recipeCategoryIds.Contains(category.Id))
                                                            .CountAsync(cancellationToken) == categoryIds.Count();
                }).WithMessage("Some of recipe categories does not exist");

            RuleFor(x => x.IngredientId)
                .MustAsync(async (ingredientId, cancellationToken) =>
                {
                    if (ingredientId is null)
                    {
                        return true;
                    }

                    IngredientId id = new(ingredientId.Value);

                    return await dbContext.Ingredients.AsNoTracking()
                                                      .AnyAsync(ingredient => ingredient.Id == id, cancellationToken);
                }).WithMessage("Provided invalid IngredientId for recipe")
                .Must((request, ingredientId) =>
                {
                    if (ingredientId is null)
                    {
                        return true;
                    }

                    return !request.Ingredients.Any(ingredient => ingredient.Ingredient.Id == ingredientId.Value);
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
                .MustAsync(async (ingredients, cancellationToken) =>
                {
                    IEnumerable<IngredientId> ingredientIds = ingredients.Select(i => new IngredientId(i.Ingredient.Id));

                    return await dbContext.Ingredients.AsNoTracking()
                                                       .Where(ingredient => ingredientIds.Contains(ingredient.Id))
                                                       .CountAsync(cancellationToken) == ingredients.Count();
                })
                .WithMessage("Some of ingredients does not exist")
                .MustAsync(async (ingredients, cancellationToken) =>
                {
                    IEnumerable<UnitId> unitIds = ingredients.Select(i => new UnitId(i.Unit.Id)).Distinct();

                    return await dbContext.Units.AsNoTracking()
                                                .Where(unit => unitIds.Contains(unit.Id))
                                                .CountAsync(cancellationToken) == unitIds.Count();

                })
                .WithMessage("Some of units does not exist")
                .Must(ingredients =>
                {
                    return ingredients.Select(i => i.Ingredient.Id).Distinct().Count() == ingredients.Count();
                })
                .WithMessage("Duplicate ingredients are not allowed");

            RecipeAmountValidator amountValidator = new();

            RuleForEach(x => x)
                .ChildRules(ingredient =>
                {
                    ingredient.RuleFor(i => i.Amount).SetValidator(amountValidator);
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
                               step.RuleFor(s => s.Description).SetValidator(new RecipeStepDescriptionValidator());
                               step.RuleFor(s => s.ImageUrl).SetValidator(new RecipeImageUrlValidator());
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
