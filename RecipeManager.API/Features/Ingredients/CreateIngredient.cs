using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients;

public static class CreateIngredient
{
    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);

    public sealed record IngredientUnitConvertionDto(Guid UnitToConvertId, double Ratio);

    public sealed record IngredientPackageDto(Guid PackageUnitId, int PackageSize, Guid PackageSizeUnitId);

    public sealed record Request(
        string Name,
        NutritionalValueDto NutritionalValues,
        Guid BaseUnit,
        IngredientPackageDto IngredientPackage,
        IEnumerable<IngredientUnitConvertionDto> IngredientUnitConvertions,
        Guid? ShoppingListCategoryId,
        IEnumerable<Guid> Categories,
        IEnumerable<Guid> Recipes
    );

    public sealed record Response(IngredientId Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext, IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Name).ValidateIngredientName();

            RuleFor(x => x.NutritionalValues.Calories).ValidateNutritionalCalories();
            RuleFor(x => x.NutritionalValues.Carbohydrates).ValidateNutritionalCarbohydrates();
            RuleFor(x => x.NutritionalValues.Fats).ValidateNutritionalFats();
            RuleFor(x => x.NutritionalValues.Proteins).ValidateNutritionalProteins();
            RuleFor(x => x.NutritionalValues.IngredientAmount).ValidateNutritionalAmount();

            RuleFor(x => x.NutritionalValues.IngredientUnitId)
                .MustReferenceExistingUnit(unitOfWork, "Ingredient unit does not exist");

            RuleFor(x => x.BaseUnit)
                .MustReferenceExistingUnit(unitOfWork, "Base unit does not exist");

            RuleFor(x => x.IngredientPackage.PackageUnitId)
                .MustReferenceExistingUnit(unitOfWork, "Package unit does not exist");

            RuleFor(x => x.IngredientPackage.PackageSize)
                .GreaterThan(0).WithMessage("Package size must be greater than 0");

            RuleFor(x => x.IngredientPackage.PackageSizeUnitId)
                .MustReferenceExistingUnit(unitOfWork, "Package size unit does not exist");

            RuleForEach(x => x.IngredientUnitConvertions).ChildRules(convertion =>
            {
                convertion.RuleFor(c => c.UnitToConvertId)
                    .MustReferenceExistingUnit(unitOfWork, "Unit to convert does not exist");
                convertion.RuleFor(c => c.Ratio).GreaterThan(0).WithMessage("Ratio must be greater than 0");
            });

            When(x => x.ShoppingListCategoryId.HasValue, () =>
            {
                RuleFor(x => x.ShoppingListCategoryId)
                    .MustReferenceExistingIngredientCategory(dbContext, "Shopping list category does not exist")
                    .Must((request, shoppingListCategoryId) => !request.Categories.Contains(shoppingListCategoryId!.Value))
                        .WithMessage("Categories can not include the ShoppingListCategoryId.");
            });

            When(request => request.Categories.Any(), () =>
            {
                RuleFor(x => x.Categories)
                    .MustHaveUniqueIds(categoryId => categoryId, "Duplicate categories are not allowed.")
                    .MustReferenceExistingIngredientCategories(dbContext, categories => categories, "Some of categories does not exist");
            });

            When(request => request.Recipes.Any(), () =>
            {
                RuleFor(x => x.Recipes)
                    .MustHaveUniqueIds(recipeId => recipeId, "Duplicate recipes are not allowed.")
                    .MustReferenceExistingRecipes(dbContext, recipes => recipes, "Some of recipes does not exist");
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
