using FluentValidation;
using FluentValidation.Results;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.RecipeCategories.Command.CreateRecipeCategory;

public sealed record CreateRecipeCategoryRequest(string Name, int CategoryType);

public sealed record CreateRecipeCategoryCommand(CreateRecipeCategoryRequest Request);

public sealed record CreateRecipeCategoryResponse(RecipeCategoryId Id)
{
    internal static CreateRecipeCategoryResponse FromCategory(RecipeCategory category) => new(category.Id);
}

public sealed class CreateRecipeCategoryCommandHandler(
    IAppDbContext dbContext,
    IValidator<CreateRecipeCategoryRequest> validator,
    IAuthorizationPolicy<CreateRecipeCategoryRequest> authorizationPolicy)
{
    public async Task<Result<CreateRecipeCategoryResponse>> Handle(
        CreateRecipeCategoryCommand command,
        CancellationToken cancellationToken)
    {
        CreateRecipeCategoryRequest request = command.Request;
        if (!await authorizationPolicy.IsAuthorized(request))
        {
            return new AuthorizationError();
        }

        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<CreateRecipeCategoryRequest>.ToValidationError(validationResult);
        }

        RecipeCategory category = RecipeCategory.Create(request.Name, (RecipeCategoryType)request.CategoryType);
        dbContext.RecipeCategories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreateRecipeCategoryResponse.FromCategory(category);
    }
}
