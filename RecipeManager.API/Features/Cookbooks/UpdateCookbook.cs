using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Cookbooks;

namespace RecipeManager.Api.Features.Cookbooks;

public static class UpdateCookbook
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public record CookbookCategoryDto(string Name, IEnumerable<CookbookRecipeDto> Recipes, IEnumerable<CookbookCategoryDto> Subcategories);

    public record CookbookRecipeDto(Guid Id);

    public sealed record CookbookDto(string Title, string Description, string? CoverImageUrl, IEnumerable<CookbookCategoryDto> Categories);

    public sealed class AuthorizationPolicy(IAppDbContext dbContext, ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            CookbookId id = new(request.Id);

            return await dbContext.Cookbooks.AnyAsync(cookbook => cookbook.Id == id && cookbook.CreatedBy == currentUser.Id);
        }
    }

    public sealed class Validator : AbstractValidator<CookbookDto>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Title).SetValidator(new CookbookTitleValidator());
            RuleFor(x => x.Description).SetValidator(new CookbookDescriptionValidator());

            When(request => request.Categories.Any(), () =>
            {
                CookbookCategoryValidator categoryCategoryValidator = new(dbContext);

                RuleForEach(request => request.Categories).SetValidator(categoryCategoryValidator);
            });
        }
    }

    private static IEnumerable<string> GetCategoryNames(CookbookCategoryDto category)
    {
        List<string> names = [category.Name];

        if (category.Subcategories.Any())
        {
            names.AddRange(category.Subcategories.SelectMany(GetCategoryNames));
        }

        return names;
    }

    public sealed class CookbookCategoryValidator : AbstractValidator<CookbookCategoryDto>
    {
        public CookbookCategoryValidator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Name).SetValidator(new CookbookCategoryNameValidator());

            When(category => category.Subcategories.Any(), () =>
            {
                RuleForEach(x => x.Subcategories).SetValidator(this);
            });
        }
    }

    [GroupEndpoint("Cookbooks")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPut<Request, CookbookDto>("/{cookbookId}", Handler)
                     .WithName("UpdateCookbook")
                     .WithDescription("Updates a cookbook");
        }
    }

    public static async Task<IResult> Handler(Request cookbookId, [FromBody] CookbookDto request, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        CookbookId id = new(cookbookId.Id);

        Cookbook? cookbook = await dbContext.Cookbooks.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                          ?? throw new EntityNotFoundException<Cookbook, CookbookId>(id);

        await dbContext.CookbookCategories.Where(category => category.Cookbook.Id == cookbook.Id)
                                          .ExecuteDeleteAsync(cancellationToken);

        cookbook.Update(request.Title, request.Description, request.CoverImageUrl, request.Categories.Select(category => MapToCookbookCategory(category, cookbook)));
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }

    private static CookbookCategory MapToCookbookCategory(CookbookCategoryDto categoryDto, Cookbook cookbook)
    {
        List<CookbookCategory> subcategories = categoryDto.Subcategories.Select(subcategoryDto => MapToCookbookCategory(subcategoryDto, cookbook)).ToList();

        return CookbookCategory.Create(categoryDto.Name, cookbook, subcategories, categoryDto.Recipes.Select(recipe => new RecipeId(recipe.Id)));
    }
}
