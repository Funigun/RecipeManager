using FluentValidation;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.IngredientCategories.Command.CreateIngredientCategory;

public sealed class CreateIngredientCategoryAuthorizationPolicy(ICurrentUser currentUser)
    : IAuthorizationPolicy<CreateIngredientCategoryRequest>
{
    public Task<bool> IsAuthorized(CreateIngredientCategoryRequest request) =>
        Task.FromResult(currentUser.HasRole(UserRoles.Admin));
}

public sealed class CreateIngredientCategoryValidationPolicy : BaseValidator<CreateIngredientCategoryRequest>
{
    public CreateIngredientCategoryValidationPolicy(IAppDbContext dbContext)
    {
        RuleFor(request => request.Name)
            .ValidateIngredientCategoryName()
            .MustHaveUniqueIngredientCategoryName(dbContext, "Ingredient category must be unique");
    }
}
