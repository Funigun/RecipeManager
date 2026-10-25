using FluentValidation;
using FluentValidation.Results;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Validation;

namespace RecipeManager.Api.Features.Ingredients.Command.CreateIngredient;

public sealed record CreateIngredientNutritionalValueDto(
    int Calories,
    double Proteins,
    double Fats,
    double Carbohydrates,
    int IngredientAmount,
    Guid IngredientUnitId);

public sealed record CreateIngredientUnitConvertionDto(Guid UnitToConvertId, double Ratio);

public sealed record CreateIngredientPackageDto(Guid PackageUnitId, int PackageSize, Guid PackageSizeUnitId);

public sealed record CreateIngredientRequest(
    string Name,
    CreateIngredientNutritionalValueDto NutritionalValues,
    Guid BaseUnit,
    CreateIngredientPackageDto IngredientPackage,
    IEnumerable<CreateIngredientUnitConvertionDto> IngredientUnitConvertions,
    Guid? ShoppingListCategoryId,
    IEnumerable<Guid> Categories,
    IEnumerable<Guid> Recipes)
{
    internal Ingredient ToIngredient()
    {
        return Ingredient.Create(
            Name,
            ToNutritionalValue(NutritionalValues),
            new UnitId(BaseUnit),
            new IngredientPackage
            {
                PackageUnitId = new UnitId(IngredientPackage.PackageUnitId),
                PackageSize = IngredientPackage.PackageSize,
                PackageSizeUnitId = new UnitId(IngredientPackage.PackageSizeUnitId)
            },
            IngredientUnitConvertions.Select(conversion => new IngredientUnitConvertion
            {
                UnitToConvertId = new UnitId(conversion.UnitToConvertId),
                Ratio = conversion.Ratio
            }),
            ShoppingListCategoryId,
            Categories.Select(categoryId => new IngredientCategoryId(categoryId)),
            Recipes.Select(recipeId => new RecipeId(recipeId)));
    }

    private static NutritionalValue ToNutritionalValue(CreateIngredientNutritionalValueDto dto)
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

public sealed record CreateIngredientCommand(CreateIngredientRequest Request);

public sealed record CreateIngredientResponse(IngredientId Id);

public sealed class CreateIngredientCommandHandler(
    IAppDbContext dbContext,
    IValidator<CreateIngredientRequest> validator)
{
    public async Task<Result<CreateIngredientResponse>> Handle(
        CreateIngredientCommand command,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<CreateIngredientRequest>.ToValidationError(validationResult);
        }

        Ingredient ingredient = command.Request.ToIngredient();
        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateIngredientResponse(ingredient.Id);
    }
}
