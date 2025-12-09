using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Ingredients;

namespace RecipeManager.Api.Features.Ingredients;

public static class CreateIngredient
{
    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);

    public sealed record Request(string Name, NutritionalValueDto NutritionalValues, Guid? ShoppingListCategoryId, IEnumerable<Guid> Categories, IEnumerable<Guid> Recipes);

    public sealed record Response(IngredientId Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Name).SetValidator(new IngredientNameValidator());

            RuleFor(x => x.NutritionalValues.Calories).SetValidator(new IngredientNutritionalCaloriesValueValidator());
            RuleFor(x => x.NutritionalValues.Carbohydrates).SetValidator(new IngredientNutritionalCarbohydratesValueValidator());
            RuleFor(x => x.NutritionalValues.Fats).SetValidator(new IngredientNutritionalFatsValueValidator());
            RuleFor(x => x.NutritionalValues.Proteins).SetValidator(new IngredientNutritionalProteinsValueValidator());
            RuleFor(x => x.NutritionalValues.IngredientAmount).SetValidator(new IngredientNutritionalAmountValidator());

            RuleFor(x => x.NutritionalValues.IngredientUnitId).MustAsync(async (ingredientUnitId, cancellationToken) =>
            {
                UnitId unitId = new(ingredientUnitId);
                return await dbContext.Units.AsNoTracking().AnyAsync(unit => unit.Id == unitId, cancellationToken);
            }).WithMessage("Ingredient unit does not exist");

            When(x => x.ShoppingListCategoryId.HasValue, () =>
            {
                RuleFor(x => x.ShoppingListCategoryId)
                    .MustAsync(async (shoppingListCategoryId, cancellationToken) =>
                    {
                        IngredientCategoryId ingredientCategoryId = new(shoppingListCategoryId!.Value);
                        return await dbContext.IngredientCategories.AsNoTracking()
                                                                   .AnyAsync(category => category.Id == ingredientCategoryId, cancellationToken);
                    }).WithMessage("Shopping list category does not exist")
                    .Must((request, shoppingListCategoryId) => !request.Categories.Contains(shoppingListCategoryId!.Value))
                        .WithMessage("Categories can not include the ShoppingListCategoryId.");
            });

            When(request => request.Categories.Any(), () =>
            {
                RuleFor(x => x.Categories)
                    .MustAsync(async (categories, cancellationToken) =>
                    {
                        IEnumerable<IngredientCategoryId> categoryIds = categories.Select(c => new IngredientCategoryId(c));

                        return await dbContext.IngredientCategories.AsNoTracking()
                                                                   .Where(category => categoryIds.Contains(category.Id))
                                                                   .CountAsync(cancellationToken) == categories.Count();
                    }).WithMessage("Some of categories does not exist");
            });

            When(request => request.Recipes.Any(), () =>
            {
                RuleFor(x => x.Recipes)
                    .MustAsync(async (recipes, cancellationToken) =>
                    {
                        IEnumerable<RecipeId> recipeIds = recipes.Select(r => new RecipeId(r));

                        return await dbContext.Recipes.AsNoTracking()
                                                      .Where(recipe => recipeIds.Contains(recipe.Id))
                                                      .CountAsync(cancellationToken) == recipes.Count();
                    }).WithMessage("Some of recipes does not exist");
            });
        }
    }

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardValidatedPost<Request, Response>(string.Empty, Handler)
                     .WithName("CreateIngredient")
                     .WithDescription("Creates a new ingredient");
        }
    }

    public static async Task<IResult> Handler([FromBody] Request request, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Ingredient ingredient = request.ToIngredient();

        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/ingredients/{ingredient.Id}", new Response(ingredient.Id));
    }

    private static Ingredient ToIngredient(this Request request)
    {
        return Ingredient.Create
        (
            request.Name,
            request.NutritionalValues.ToNutritionalValue(),
            request.ShoppingListCategoryId,
            request.Categories.Select(c => new IngredientCategoryId(c)),
            request.Recipes.Select(r => new RecipeId(r))
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
