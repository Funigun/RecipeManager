using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Cookbooks.Command.UpdateCookbook;

public sealed class UpdateCookbookAuthorizationPolicy(IAppDbContext dbContext, ICurrentUser currentUser)
    : IAuthorizationPolicy<UpdateCookbookCommand>
{
    public async Task<bool> IsAuthorized(UpdateCookbookCommand command)
    {
        CookbookId id = new(command.CookbookId);
        return await dbContext.Cookbooks.AnyAsync(
            cookbook => cookbook.Id == id && cookbook.CreatedBy == currentUser.Id);
    }
}

public sealed class UpdateCookbookValidationPolicy : BaseValidator<UpdateCookbookRequest>
{
    public UpdateCookbookValidationPolicy()
    {
        RuleFor(request => request.Title).ValidateCookbookTitle();
        RuleFor(request => request.Description).ValidateCookbookDescription();
        RuleFor(request => request.Categories)
            .ValidateCookbookCategoryNames(category => category.Name, category => category.Subcategories);
    }
}
