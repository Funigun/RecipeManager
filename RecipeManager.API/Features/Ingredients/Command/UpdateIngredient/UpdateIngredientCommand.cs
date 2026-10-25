using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Ingredients.Command.UpdateIngredient;

public sealed record UpdateIngredientNutritionalValueDto(
    int Calories,
    double Proteins,
    double Fats,
    double Carbohydrates,
    int IngredientAmount,
    Guid IngredientUnitId);

public sealed record UpdateIngredientUnitConvertionDto(Guid UnitToConvertId, double Ratio);

public sealed record UpdateIngredientPackageDto(Guid PackageUnitId, int PackageSize, Guid PackageSizeUnitId);

public sealed record UpdateIngredientRequest(
    string Name,
    UpdateIngredientNutritionalValueDto NutritionalValues,
    Guid BaseUnit,
    UpdateIngredientPackageDto IngredientPackage,
    IEnumerable<UpdateIngredientUnitConvertionDto> IngredientUnitConvertions,
    Guid? ShoppingListCategoryId,
    IEnumerable<Guid> Categories,
    IEnumerable<Guid> Recipes)
{
    internal void ApplyTo(Ingredient ingredient)
    {
        ingredient.Update(
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

    private static NutritionalValue ToNutritionalValue(UpdateIngredientNutritionalValueDto dto)
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

public sealed record UpdateIngredientCommand(Guid IngredientId, UpdateIngredientRequest Request);

public sealed class UpdateIngredientCommandHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IValidator<UpdateIngredientRequest> validator,
    IAuthorizationPolicy<UpdateIngredientCommand> authorizationPolicy)
{
    public async Task<Result<IngredientId>> Handle(
        UpdateIngredientCommand command,
        CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        IngredientId id = new(command.IngredientId);
        Ingredient? ingredient = await dbContext.Ingredients
            .Include(item => item.IngredientPackage)
            .Include(item => item.Categories)
            .Include(item => item.Recipes)
            .Include(item => item.IngredientUnitConvertions)
            .FirstOrDefaultAsync(
                item => item.Id == id && item.CreatedBy == currentUser.Id,
                cancellationToken);
        if (ingredient is null)
        {
            return new NotFoundError();
        }

        ValidationResult validationResult = await validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<UpdateIngredientRequest>.ToValidationError(validationResult);
        }

        command.Request.ApplyTo(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.Id;
    }
}
