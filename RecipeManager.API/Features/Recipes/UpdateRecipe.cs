using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Recipes;

namespace RecipeManager.Api.Features.Recipes;

public static class UpdateRecipe
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed record RecipeIngredientDto(Guid IngredientId, Guid UnitId, double Amount);

    public sealed record RecipeAmountDto(double Value, Guid UnitId);

    public sealed record RecipeSectionDto(int SectionType, IEnumerable<RecipeStepDto> Steps);

    public sealed record RecipeStepDto(int Order, string Description, string? ImageUrl);

    public sealed record RecipeDto(string Title, string Description, string? ImageUrl, string? VideoUrl, RecipeAmountDto Amount, byte NumberOfServings, int Difficulty,
                                   IEnumerable<RecipeIngredientDto> Ingredients, IEnumerable<RecipeSectionDto> Sections, IEnumerable<Guid> CategoryIds, Guid? IngredientId);

    public sealed class Validator : AbstractValidator<RecipeDto>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Title).SetValidator(new RecipeTitleValidator());
            RuleFor(x => x.Description).SetValidator(new RecipeDescriptionValidator());
            RuleFor(x => x.ImageUrl).SetValidator(new RecipeImageUrlValidator());
            RuleFor(x => x.VideoUrl).SetValidator(new RecipeVideoUrlValidator());
            RuleFor(x => x.Amount.Value).SetValidator(new RecipeAmountValidator());
            RuleFor(x => x.Amount.UnitId).NotEmpty()
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

            RuleFor(x => x.Ingredients).SetValidator(new RecipeIngredientsValidator(dbContext));
            RuleFor(x => x.Sections).SetValidator(new RecipeSectionsValidator());

            RuleFor(x => x.CategoryIds)
                .Must(ids => ids.Distinct().Count() == ids.Count())
                    .WithMessage("Duplicate categories are not allowed.")
                .MustAsync(async (categoryIds, cancellationToken) =>
                {
                    if (!categoryIds.Any())
                    {
                        return true;
                    }

                    IEnumerable<RecipeCategoryId> recipeCategoryIds = categoryIds.Select(c => new RecipeCategoryId(c));

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

                    return !request.Ingredients.Any(ingredient => ingredient.IngredientId == ingredientId.Value);
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
                    IEnumerable<IngredientId> ingredientIds = ingredients.Select(i => new IngredientId(i.IngredientId));

                    return await dbContext.Ingredients.AsNoTracking()
                                                       .Where(ingredient => ingredientIds.Contains(ingredient.Id))
                                                       .CountAsync(cancellationToken) == ingredients.Count();
                })
                .WithMessage("Some of ingredients does not exist")
                .MustAsync(async (ingredients, cancellationToken) =>
                {
                    IEnumerable<UnitId> unitIds = ingredients.Select(i => new UnitId(i.UnitId));

                    return await dbContext.Units.AsNoTracking()
                                                 .Where(unit => unitIds.Contains(unit.Id))
                                                 .CountAsync(cancellationToken) == ingredients.Count();

                })
                .WithMessage("Some of units does not exist")
                .Must(ingredients =>
                {
                    return ingredients.Select(i => i.IngredientId).Distinct().Count() == ingredients.Count();
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
            endpoints.MapStandardValidatedPut<Request>("/{id}", Handler)
                     .WithName("UpdateRecipe")
                     .WithDescription("Updates existing recipe");
        }
    }

    public static async Task<IResult> Handler(Request request, RecipeDto recipeDto, IAppDbContext dbContext, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        RecipeId recipeId = new(request.Id);
        Recipe recipe = await dbContext.Recipes.AsNoTracking()
                                               .FirstOrDefaultAsync(r => r.Id == recipeId && r.CreatedBy == currentUser.Id, cancellationToken)
                     ?? throw new EntityNotFoundException<Recipe, RecipeId>(recipeId);



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
        recipe.IngredientId = recipeDto.IngredientId.HasValue ? new IngredientId(recipeDto.IngredientId.Value) : null;
        recipe.Ingredients = recipeDto.Ingredients.Select(i => i.ToDomain()).ToList();
        recipe.Sections = recipeDto.Sections.Select(s => s.ToDomain()).ToList();
        recipe.Categories = recipeDto.CategoryIds.Select(categoryId => new RecipeCategoryId(categoryId)).ToList();
    }

    private static RecipeAmount ToDomain(this RecipeAmountDto dto)
    {
        return new(dto.Value, new UnitId(dto.UnitId));
    }

    private static RecipeIngredient ToDomain(this RecipeIngredientDto dto)
    {
        return RecipeIngredient.Create
        (
            new IngredientId(dto.IngredientId),
            new UnitId(dto.UnitId),
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
}
