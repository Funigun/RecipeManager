using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Cookbooks;

public static class CreateCookbook
{
    public record CookbookCategoryDto(string Name, IEnumerable<CookbookRecipeDto> Recipes, IEnumerable<CookbookCategoryDto> Subcategories);

    public record CookbookRecipeDto(Guid Id);

    public sealed record Request(string Title, string Description, string? CoverImageUrl, IEnumerable<CookbookCategoryDto> Categories);

    public sealed record Response(Guid Id);

    // ToDo: validate categories unique name within the same level
    // ToDo: validate recipe ids within whole cookbook
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Title).ValidateCookbookTitle();
            RuleFor(x => x.Description).ValidateCookbookDescription();

            RuleFor(x => x.Categories)
                .ValidateCookbookCategoryNames(category => category.Name, category => category.Subcategories);
        }
    }

    [GroupEndpoint("Cookbooks")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardValidatedPost<Request, Response>(string.Empty, Handler)
                     .WithName("CreateCookbook")
                     .WithDescription("Creates a new cookbook");
        }
    }

    public static async Task<Response> Handler([FromBody] Request request, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Cookbook cookbook = Cookbook.Create(request.Title, request.Description, request.CoverImageUrl, []);
        List<CookbookCategory> categories = request.Categories.Select(categoryDto => MapToCookbookCategory(categoryDto, cookbook)).ToList();
        cookbook.Update(request.Title, request.Description, request.CoverImageUrl, categories);

        dbContext.Cookbooks.Add(cookbook);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Response(cookbook.Id.Value);
    }

    private static CookbookCategory MapToCookbookCategory(CookbookCategoryDto categoryDto, Cookbook cookbook)
    {
        List<CookbookCategory> subcategories = categoryDto.Subcategories.Select(subcategoryDto => MapToCookbookCategory(subcategoryDto, cookbook)).ToList();

        return CookbookCategory.Create(categoryDto.Name, cookbook, subcategories, categoryDto.Recipes.Select(recipe => new RecipeId(recipe.Id)));
    }
}
