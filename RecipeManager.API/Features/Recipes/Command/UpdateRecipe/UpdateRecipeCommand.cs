using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Recipes.Command.UpdateRecipe;

public sealed record UpdateRecipeIngredientDto(UpdateRecipeIngredientIdDto Ingredient, UpdateRecipeUnitDto Unit, double Amount);
public sealed record UpdateRecipeIngredientIdDto(Guid Id, Guid? IngredientRecipe);
public sealed record UpdateRecipeUnitDto(Guid Id);
public sealed record UpdateRecipeAmountDto(double Amount, UpdateRecipeUnitDto Unit);
public sealed record UpdateRecipeNutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);
public sealed record UpdateRecipeSectionDto(int SectionType, IEnumerable<UpdateRecipeStepDto> Steps);
public sealed record UpdateRecipeStepDto(int Order, string Description, string? ImageUrl);
public sealed record UpdateRecipeCategoryDto(Guid Id);
public sealed record UpdateRecipeRequest(
    string Title,
    string Description,
    string? ImageUrl,
    string? VideoUrl,
    UpdateRecipeAmountDto Amount,
    byte NumberOfServings,
    int Difficulty,
    UpdateRecipeNutritionalValueDto NutritionalValues,
    IEnumerable<UpdateRecipeIngredientDto> Ingredients,
    IEnumerable<UpdateRecipeSectionDto> Sections,
    IEnumerable<UpdateRecipeCategoryDto> Categories,
    Guid? IngredientId);
public sealed record UpdateRecipeCommand(Guid RecipeId, UpdateRecipeRequest Request);
internal sealed record UpdateRecipeProjection(Recipe Recipe);
public sealed record UpdateRecipeResponse(RecipeId Id);

public sealed class UpdateRecipeCommandHandler(
    IAppDbContext dbContext,
    IValidator<UpdateRecipeRequest> validator,
    IAuthorizationPolicy<UpdateRecipeCommand> authorizationPolicy,
    ICurrentUser currentUser)
{
    public async Task<Result<UpdateRecipeResponse>> Handle(UpdateRecipeCommand command, CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        RecipeId id = new(command.RecipeId);
        Recipe? recipe = await dbContext.Recipes
            .Include(item => item.Ingredients)
            .Include(item => item.Sections).ThenInclude(section => section.Steps)
            .Include(item => item.Categories)
            .FirstOrDefaultAsync(item => item.Id == id && item.CreatedBy == currentUser.Id, cancellationToken);
        if (recipe is null)
        {
            return new NotFoundError();
        }

        ValidationResult validationResult = await validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<UpdateRecipeRequest>.ToValidationError(validationResult);
        }

        UpdateRecipeMappings.ApplyTo(recipe, command.Request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateRecipeResponse(recipe.Id);
    }
}

internal static class UpdateRecipeMappings
{
    internal static void ApplyTo(Recipe recipe, UpdateRecipeRequest request)
    {
        recipe.Title = request.Title;
        recipe.Description = request.Description;
        recipe.ImageURL = request.ImageUrl;
        recipe.VideoURL = request.VideoUrl;
        recipe.Amount = new RecipeAmount(request.Amount.Amount, new UnitId(request.Amount.Unit.Id));
        recipe.NumberOfServings = request.NumberOfServings;
        recipe.Difficulty = (RecipeDifficulty)request.Difficulty;
        recipe.NutritionalValue = request.NutritionalValues.ToDomain();
        recipe.IngredientId = request.IngredientId.HasValue ? new IngredientId(request.IngredientId.Value) : null;
        recipe.Ingredients = request.Ingredients.Select(item => RecipeIngredient.Create(
            new IngredientId(item.Ingredient.Id), new UnitId(item.Unit.Id), item.Amount)).ToList();
        recipe.Sections = request.Sections.Select(section => RecipeSection.Create(
            section.SectionType,
            section.Steps.Select(step => RecipeStep.Create(step.Order, step.Description, step.ImageUrl)).ToList())).ToList();
        recipe.UpdateCategories(request.Categories.Select(category => new RecipeCategoryId(category.Id)).ToList());
    }

    private static NutritionalValue ToDomain(this UpdateRecipeNutritionalValueDto dto) => new()
    {
        Calories = dto.Calories,
        Proteins = dto.Proteins,
        Fats = dto.Fats,
        Carbohydrates = dto.Carbohydrates,
        IngredientAmount = dto.IngredientAmount,
        IngredientUnit = new UnitId(dto.IngredientUnitId)
    };
}
