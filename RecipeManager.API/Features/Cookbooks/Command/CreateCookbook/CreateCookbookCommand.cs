using FluentValidation;
using FluentValidation.Results;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Validation;

namespace RecipeManager.Api.Features.Cookbooks.Command.CreateCookbook;

public sealed record CreateCookbookCategoryDto(
    string Name,
    IEnumerable<CreateCookbookRecipeDto> Recipes,
    IEnumerable<CreateCookbookCategoryDto> Subcategories);

public sealed record CreateCookbookRecipeDto(Guid Id);

public sealed record CreateCookbookRequest(
    string Title,
    string Description,
    string? CoverImageUrl,
    IEnumerable<CreateCookbookCategoryDto> Categories);

public sealed record CreateCookbookCommand(CreateCookbookRequest Request);

public sealed record CreateCookbookResponse(Guid Id);

public sealed class CreateCookbookCommandHandler(
    IAppDbContext dbContext,
    IValidator<CreateCookbookRequest> validator)
{
    public async Task<Result<CreateCookbookResponse>> Handle(CreateCookbookCommand command, CancellationToken cancellationToken)
    {
        CreateCookbookRequest request = command.Request;
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<CreateCookbookRequest>.ToValidationError(validationResult);
        }

        Cookbook cookbook = Cookbook.Create(request.Title, request.Description, request.CoverImageUrl, []);
        List<CookbookCategory> categories = request.Categories.Select(category => MapToCookbookCategory(category, cookbook)).ToList();
        cookbook.Update(request.Title, request.Description, request.CoverImageUrl, categories);

        dbContext.Cookbooks.Add(cookbook);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCookbookResponse(cookbook.Id.Value);
    }

    private static CookbookCategory MapToCookbookCategory(CreateCookbookCategoryDto categoryDto, Cookbook cookbook)
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
