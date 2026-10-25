using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Cookbooks.Command.UpdateCookbook;

public sealed record UpdateCookbookCategoryDto(
    string Name,
    IEnumerable<UpdateCookbookRecipeDto> Recipes,
    IEnumerable<UpdateCookbookCategoryDto> Subcategories);

public sealed record UpdateCookbookRecipeDto(Guid Id);

public sealed record UpdateCookbookRequest(
    string Title,
    string Description,
    string? CoverImageUrl,
    IEnumerable<UpdateCookbookCategoryDto> Categories);

public sealed record UpdateCookbookCommand(Guid CookbookId, UpdateCookbookRequest Request);

public sealed class UpdateCookbookCommandHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IValidator<UpdateCookbookRequest> validator,
    IAuthorizationPolicy<UpdateCookbookCommand> authorizationPolicy)
{
    public async Task<Result<Guid>> Handle(UpdateCookbookCommand command, CancellationToken cancellationToken)
    {
        if (!await authorizationPolicy.IsAuthorized(command))
        {
            return new AuthorizationError();
        }

        CookbookId id = new(command.CookbookId);
        Cookbook? cookbook = await dbContext.Cookbooks.FirstOrDefaultAsync(
            item => item.Id == id && item.CreatedBy == currentUser.Id,
            cancellationToken);
        if (cookbook is null)
        {
            return new NotFoundError();
        }

        ValidationResult validationResult = await validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<UpdateCookbookRequest>.ToValidationError(validationResult);
        }

        await dbContext.CookbookCategories
            .Where(category => category.Cookbook.Id == cookbook.Id)
            .ExecuteDeleteAsync(cancellationToken);

        cookbook.Update(
            command.Request.Title,
            command.Request.Description,
            command.Request.CoverImageUrl,
            command.Request.Categories.Select(category => MapToCookbookCategory(category, cookbook)));
        await dbContext.SaveChangesAsync(cancellationToken);

        return cookbook.Id.Value;
    }

    private static CookbookCategory MapToCookbookCategory(UpdateCookbookCategoryDto categoryDto, Cookbook cookbook)
    {
        List<CookbookCategory> subcategories = categoryDto.Subcategories
            .Select(subcategory => MapToCookbookCategory(subcategory, cookbook))
            .ToList();

        return CookbookCategory.Create(
            categoryDto.Name,
            cookbook,
            subcategories,
            categoryDto.Recipes.Select(recipe => new RecipeId(recipe.Id)));
    }
}
