using FluentValidation;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Validation;

namespace RecipeManager.Api.Features.Recipes.Command.CreateRecipe;

public sealed record CreateRecipeIngredientDto(CreateRecipeIngredientIdDto Ingredient, CreateRecipeUnitDto Unit, double Amount);
public sealed record CreateRecipeIngredientIdDto(Guid Id, Guid? IngredientRecipe);
public sealed record CreateRecipeUnitDto(Guid Id);
public sealed record CreateRecipeAmountDto(double Amount, CreateRecipeUnitDto Unit);
public sealed record CreateRecipeNutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);
public sealed record CreateRecipeSectionDto(int SectionType, IEnumerable<CreateRecipeStepDto> Steps);
public sealed record CreateRecipeStepDto(int Order, string Description, string? ImageUrl);
public sealed record CreateRecipeCategoryDto(Guid Id);
public sealed record CreateRecipeRequest(
    string Title,
    string Description,
    string? ImageUrl,
    string? VideoUrl,
    CreateRecipeAmountDto Amount,
    byte NumberOfServings,
    int Difficulty,
    CreateRecipeNutritionalValueDto NutritionalValues,
    IEnumerable<CreateRecipeIngredientDto> Ingredients,
    IEnumerable<CreateRecipeSectionDto> Sections,
    IEnumerable<CreateRecipeCategoryDto> Categories,
    Guid? IngredientId);
public sealed record CreateRecipeCommand(CreateRecipeRequest Request);
internal sealed record CreateRecipeProjection(Guid Id);
public sealed record CreateRecipeResponse(Guid Id);

public sealed class CreateRecipeCommandHandler(IAppDbContext dbContext, IValidator<CreateRecipeRequest> validator)
{
    public async Task<Result<CreateRecipeResponse>> Handle(CreateRecipeCommand command, CancellationToken cancellationToken)
    {
        CreateRecipeRequest request = command.Request;
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<CreateRecipeRequest>.ToValidationError(validationResult);
        }

        Recipe recipe = request.ToDomain();
        dbContext.Recipes.Add(recipe);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateRecipeResponse(recipe.Id.Value);
    }
}

internal static class CreateRecipeMappings
{
    internal static Recipe ToDomain(this CreateRecipeRequest request) =>
        Recipe.Create(
            request.Title,
            new RecipeAmount(request.Amount.Amount, new UnitId(request.Amount.Unit.Id)),
            request.NumberOfServings,
            (RecipeDifficulty)request.Difficulty,
            request.NutritionalValues.ToDomain(),
            request.Ingredients.Select(ingredient => RecipeIngredient.Create(
                new IngredientId(ingredient.Ingredient.Id), new UnitId(ingredient.Unit.Id), ingredient.Amount)),
            request.Sections.Select(section => RecipeSection.Create(
                section.SectionType,
                section.Steps.Select(step => RecipeStep.Create(step.Order, step.Description, step.ImageUrl)).ToList())),
            request.Categories.Select(category => new RecipeCategoryId(category.Id)),
            request.IngredientId is Guid id ? new IngredientId(id) : null!,
            request.Description,
            request.ImageUrl,
            request.VideoUrl);

    private static NutritionalValue ToDomain(this CreateRecipeNutritionalValueDto dto) => new()
    {
        Calories = dto.Calories,
        Proteins = dto.Proteins,
        Fats = dto.Fats,
        Carbohydrates = dto.Carbohydrates,
        IngredientAmount = dto.IngredientAmount,
        IngredientUnit = new UnitId(dto.IngredientUnitId)
    };
}
