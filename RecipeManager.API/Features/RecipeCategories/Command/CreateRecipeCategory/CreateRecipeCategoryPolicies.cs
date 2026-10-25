using FluentValidation;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.RecipeCategories.Command.CreateRecipeCategory;

public sealed class CreateRecipeCategoryAuthorizationPolicy(ICurrentUser currentUser)
    : IAuthorizationPolicy<CreateRecipeCategoryRequest>
{
    public Task<bool> IsAuthorized(CreateRecipeCategoryRequest request) =>
        Task.FromResult(currentUser.HasRole(UserRoles.Admin));
}

public sealed class CreateRecipeCategoryValidationPolicy : BaseValidator<CreateRecipeCategoryRequest>
{
    public CreateRecipeCategoryValidationPolicy(IAppDbContext dbContext)
    {
        RuleFor(request => request.Name)
            .ValidateRecipeCategoryName()
            .MustHaveUniqueRecipeCategoryName(dbContext, "Recipe category must be unique");

        RuleFor(request => request.CategoryType)
            .Must(categoryType => categoryType.IsRecipeCategoryType())
            .WithMessage("Provided invalid recipe category type");
    }
}
