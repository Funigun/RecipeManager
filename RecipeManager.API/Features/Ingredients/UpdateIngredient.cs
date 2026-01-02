using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Ingredients;

namespace RecipeManager.Api.Features.Ingredients;

public static class UpdateIngredient
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);

    public sealed record IngredientUnitConvertionDto(Guid UnitToConvertId, double Ratio);

    public sealed record IngredientPackageDto(Guid PackageUnitId, int PackageSize, Guid PackageSizeUnitId);

    public sealed record IngredientDto(
        string Name,
        NutritionalValueDto NutritionalValues,
        Guid BaseUnit,
        IngredientPackageDto IngredientPackage,
        IEnumerable<IngredientUnitConvertionDto> IngredientUnitConvertions,
        Guid? ShoppingListCategoryId,
        IEnumerable<Guid> Categories,
        IEnumerable<Guid> Recipes
    );

    public sealed class AuthorizationPolicy(IAppDbContext appDbContext, ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            return await appDbContext.Ingredients.AsNoTracking()
                                                 .AnyAsync(ingredient => ingredient.Id == new IngredientId(request.Id) &&
                                                           ingredient.CreatedBy == currentUser.Id);
        }
    }

    public sealed class Validator : AbstractValidator<IngredientDto>
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

            RuleFor(x => x.BaseUnit).MustAsync(async (baseUnitId, cancellationToken) =>
            {
                UnitId unitId = new(baseUnitId);
                return await dbContext.Units.AsNoTracking().AnyAsync(unit => unit.Id == unitId, cancellationToken);
            }).WithMessage("Base unit does not exist");

            RuleFor(x => x.IngredientPackage.PackageUnitId)
                .MustAsync(async (unitId, cancellationToken) => await dbContext.Units.AnyAsync(u => u.Id == new UnitId(unitId), cancellationToken))
                .WithMessage("Package unit does not exist");

            RuleFor(x => x.IngredientPackage.PackageSize)
                .GreaterThan(0).WithMessage("Package size must be greater than 0");

            RuleFor(x => x.IngredientPackage.PackageSizeUnitId)
                .MustAsync(async (unitId, cancellationToken) => await dbContext.Units.AnyAsync(u => u.Id == new UnitId(unitId), cancellationToken))
                .WithMessage("Package size unit does not exist");

            RuleForEach(x => x.IngredientUnitConvertions).ChildRules(convertion =>
            {
                convertion.RuleFor(c => c.UnitToConvertId).MustAsync(async (unitToConvertId, cancellationToken) =>
                {
                    UnitId unitId = new(unitToConvertId);
                    return await dbContext.Units.AsNoTracking().AnyAsync(unit => unit.Id == unitId, cancellationToken);
                }).WithMessage("Unit to convert does not exist");
                convertion.RuleFor(c => c.Ratio).GreaterThan(0).WithMessage("Ratio must be greater than 0");
            });

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
            endpoints.MapStandardAuthenticatedPut<Request, IngredientDto>("/{ingredientId}", Handler)
                     .WithName("UpdateIngredient")
                     .WithDescription("Updates an existing ingredient");
        }
    }

    public static async Task<IResult> Handler(Request ingredientId, [FromBody] IngredientDto request, [FromServices] IAppDbContext dbContext, [FromServices] ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        IngredientId id = new(ingredientId.Id);

        Ingredient? ingredient = await dbContext.Ingredients
                                                .Include(i => i.IngredientPackage)
                                                .Include(i => i.Categories)
                                                .Include(i => i.Recipes)
                                                .Include(i => i.IngredientUnitConvertions)
                                                .FirstOrDefaultAsync(i => i.Id == id && i.CreatedBy == currentUser.Id, cancellationToken)
                              ?? throw new EntityNotFoundException<Ingredient, IngredientId>(id);

        ingredient.Update
        (
            request.Name,
            request.NutritionalValues.ToNutritionalValue(),
            new UnitId(request.BaseUnit),
            new IngredientPackage
            {
                PackageUnitId = new UnitId(request.IngredientPackage.PackageUnitId),
                PackageSize = request.IngredientPackage.PackageSize,
                PackageSizeUnitId = new UnitId(request.IngredientPackage.PackageSizeUnitId)
            },
            request.IngredientUnitConvertions.Select(c => new IngredientUnitConvertion { UnitToConvertId = new UnitId(c.UnitToConvertId), Ratio = c.Ratio }),
            request.ShoppingListCategoryId,
            request.Categories.Select(c => new IngredientCategoryId(c)),
            request.Recipes.Select(r => new RecipeId(r))
        );

        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
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
