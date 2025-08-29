using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Cookbooks;

namespace RecipeManager.Api.Features.Cookbooks;

public static class UpdateCookbook
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public record CookbookCategoryDto(string Name, IEnumerable<Guid> Recipes, IEnumerable<CookbookCategoryDto> Subcategories);

    public sealed record CookbookDto(string Title, IEnumerable<CookbookCategoryDto> Categories);

    public sealed class AuthorizationPolicy(IAppDbContext dbContext, ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            CookbookId id = new(request.Id);

            return await dbContext.Cookbooks.AnyAsync(cookbook => cookbook.Id == id && cookbook.CreatedBy == currentUser.Id);
        }
    }

    // ToDo: validate categories unique name within the same level
    // ToDo: validate recipe ids within whole cookbook
    public sealed class Validator : AbstractValidator<CookbookDto>
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
            endpoints.MapStandardAuthenticatedPut<Request, CookbookDto>("/{cookbookId}", Handler)
                     .WithName("UpdateCookbook")
                     .WithDescription("Updates a cookbook");
        }
    }

    // ToDo: Update CookbookCategory to store recipe Ids
    // ToDo: Add CookbookCategoryDto mapping to CookbookCategory
    public static async Task<IResult> Handler(Request cookbookId, CookbookDto request, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        CookbookId id = new(cookbookId.Id);

        Cookbook? cookbook = await dbContext.Cookbooks.Include(c => c.Categories)
                                                      .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                          ?? throw new EntityNotFoundException<Cookbook, CookbookId>(id);

        dbContext.CookbookCategories.RemoveRange(cookbook.Categories);
        await dbContext.SaveChangesAsync(cancellationToken);

        cookbook.Update(request.Title, []);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
