using FluentValidation;
using FluentValidation.Results;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.IngredientCategories.Command.CreateIngredientCategory;

public sealed record CreateIngredientCategoryRequest(string Name);

public sealed record CreateIngredientCategoryCommand(CreateIngredientCategoryRequest Request);

public sealed record CreateIngredientCategoryResponse(IngredientCategoryId Id)
{
    internal static CreateIngredientCategoryResponse FromCategory(IngredientCategory category) => new(category.Id);
}

public sealed class CreateIngredientCategoryCommandHandler(
    IAppDbContext dbContext,
    IValidator<CreateIngredientCategoryRequest> validator,
    IAuthorizationPolicy<CreateIngredientCategoryRequest> authorizationPolicy)
{
    public async Task<Result<CreateIngredientCategoryResponse>> Handle(
        CreateIngredientCategoryCommand command,
        CancellationToken cancellationToken)
    {
        CreateIngredientCategoryRequest request = command.Request;

        if (!await authorizationPolicy.IsAuthorized(request))
        {
            return new AuthorizationError();
        }

        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<CreateIngredientCategoryRequest>.ToValidationError(validationResult);
        }

        IngredientCategory category = IngredientCategory.Create(request.Name);
        dbContext.IngredientCategories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateIngredientCategoryResponse.FromCategory(category);
    }
}
