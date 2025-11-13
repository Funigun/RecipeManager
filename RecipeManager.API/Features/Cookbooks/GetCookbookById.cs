using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Cookbooks;

public static class GetCookbookById
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed record RecipeDto(Guid Id, string Title, string? Description, string? ImageUrl);

    public sealed record class CookbookCategoryDto(Guid Id, string Name, IEnumerable<HateoasResponse<RecipeDto>> Recipes, IEnumerable<CookbookCategoryDto> Subcategories);

    public sealed record Response(Guid Id, string Title, string Description, string? CoverImageUrl, IEnumerable<CookbookCategoryDto> Categories);

    [GroupEndpoint("Cookbooks")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint([FromServices] IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("{cookbookId}", Handler)
                     .WithName("GetCookbookById")
                     .WithDescription("Gets a cookbook by its id");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(Request cookbookId, [FromServices] IAppDbContext dbContext, [FromServices] ICurrentUser currentUser, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        Cookbook? cookbook = await GetCookbook(cookbookId, dbContext, cancellationToken);
        IEnumerable<CookbookCategory> categories = await GetCookbookCategories(cookbook.Id, dbContext, cancellationToken);
        IEnumerable<RecipeDto> recipes = await GetRecipes(categories, dbContext, cancellationToken);

        bool isCookbookCreator = cookbook.CreatedBy == currentUser.Id;

        Response response = MapToResponse(cookbook, categories, recipes, hateoasBuilderFactory);
        HateoasResponseBuilder<Response> builder = hateoasBuilderFactory.ForItem(response);

        builder.AddDelete(LinkOptions.Create("DeleteCookbook", HateoasRelConstants.Delete, isCookbookCreator), new { CookbookId = cookbook.Id });
        builder.AddPut(LinkOptions.Create("UpdateCookbook", HateoasRelConstants.Update, isCookbookCreator), new { CookbookId = cookbook.Id });

        return TypedResults.Ok(builder.Build());
    }

    private static async Task<Cookbook> GetCookbook(Request cookbookId, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        CookbookId id = new(cookbookId.Id);

        return await dbContext.Cookbooks
                              .AsNoTracking()
                              .Where(c => c.Id == id)
                              .FirstOrDefaultAsync(cancellationToken)
               ?? throw new EntityNotFoundException<Cookbook, CookbookId>(id);
    }

    private static async Task<IEnumerable<CookbookCategory>> GetCookbookCategories(CookbookId cookbookId, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.CookbookCategories
                              .AsNoTracking()
                              .Where(category => category.Cookbook.Id == cookbookId)
                              .Include(c => c.Recipes)
                              .ToListAsync(cancellationToken);
    }

    private static async Task<IEnumerable<RecipeDto>> GetRecipes(IEnumerable<CookbookCategory> categories, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IEnumerable<RecipeId> recipeIds = categories.SelectMany(c => c.Recipes).Distinct();

        return await dbContext.Recipes
                              .AsNoTracking()
                              .Where(recipe => recipeIds.Contains(recipe.Id))
                              .Select(recipe => new RecipeDto(recipe.Id.Value, recipe.Title, recipe.Description, recipe.ImageURL))
                              .ToListAsync(cancellationToken);
    }

    private static Response MapToResponse(Cookbook cookbook, IEnumerable<CookbookCategory> categories, IEnumerable<RecipeDto> recipes, IHateoasBuilderFactory hateoasBuilderFactory)
    {
        HateoasCollectionResponseBuilder<RecipeDto> recipesBuilder = hateoasBuilderFactory.ForCollection(recipes);
        recipesBuilder.WithCollectionLink()
                      .WithGet(LinkOptions.Create("GetRecipeById", HateoasRelConstants.Self, true), recipe => new { Id = recipe.Id });
        IEnumerable<HateoasResponse<RecipeDto>> hateoasRecipes = recipesBuilder.Build().Items;
        IEnumerable<CookbookCategory> rootCategories = categories.Where(c => c.ParentId is null);

        return new
        (
            cookbook.Id.Value,
            cookbook.Title,
            cookbook.Description,
            cookbook.CoverImageUrl,
            rootCategories.Select(category => MapToCookbookCategoryDto(category, categories, hateoasRecipes))
        );
    }

    private static CookbookCategoryDto MapToCookbookCategoryDto(CookbookCategory category, IEnumerable<CookbookCategory> allCategories, IEnumerable<HateoasResponse<RecipeDto>> recipes)
    {
        IEnumerable<CookbookCategory> subcategories = allCategories.Where(c => c.ParentId == category.Id);
        IEnumerable<CookbookCategoryDto> subcategoryDtos = subcategories.Select(subcategory => MapToCookbookCategoryDto(subcategory, allCategories, recipes));
        IEnumerable<HateoasResponse<RecipeDto>> categoryRecipes = recipes.Where(r => category.Recipes.Contains(new RecipeId(r.Item.Id)));

        return new CookbookCategoryDto(category.Id.Value, category.Name, categoryRecipes, subcategoryDtos);
    }
}
