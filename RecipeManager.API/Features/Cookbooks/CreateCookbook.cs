using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Cookbooks;

namespace RecipeManager.Api.Features.Cookbooks;

public static class CreateCookbook
{
    public record CookbookCategoryDto(string Name, IEnumerable<Guid> Recipes, IEnumerable<CookbookCategoryDto> Subcategories);

    public sealed record Request(string Title, IEnumerable<CookbookCategoryDto> Categories);

    public sealed record Response(Guid Id);

    // ToDo: validate categories unique name within the same level
    // ToDo: validate recipe ids within whole cookbook
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Title).SetValidator(new CookbookTitleValidator());

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

    [EndpointGroupName("Cookbooks")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardValidatedPost<Request, Response>(string.Empty, Handler)
                     .WithName("CreateCookbook")
                     .WithDescription("Creates a new cookbook");
        }
    }

    // ToDo: Update CookbookCategory to store recipe Ids
    // ToDo: Add CookbookCategoryDto mapping to CookbookCategory
    public static async Task<Response> Handler(Request request, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Cookbook? cookbook = Cookbook.Create(request.Title, []);

        dbContext.Cookbooks.Add(cookbook);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Response(cookbook.Id.Value);
    }
}
