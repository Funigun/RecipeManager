using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Recipes;

public static class GetRecipes
{
    public class ItemIds
    {
        public List<Guid> Ids = [];

        public static bool TryParse(string? value, IFormatProvider? provider, out ItemIds? articleIDs)
        {
            string? trimmedValue = value?.TrimStart('(').TrimEnd(')');
            string[]? segments = trimmedValue?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (segments == null)
            {
                articleIDs = new ItemIds();
                return false;
            }

            List<Guid>? idList = [];
            foreach (string segment in segments)
            {
                if (Guid.TryParse(segment, out Guid id))
                {
                    idList.Add(id);
                }
            }

            articleIDs = new ItemIds()
            {
                Ids = idList
            };

            return true;
        }
    }

    public sealed record GetRecipesParameters(ItemIds? Categories, ItemIds? Ingredients, int Page = 1, int PageSize = 10) : PagedParameters(Page, PageSize);

    public sealed record RecipeDto(Guid Id, string Title, string? ImageUrl, int Difficulty, byte NumberOfServings);

    public sealed record Response(int Page, int PageSize, int TotalCount, IEnumerable<HateoasResponse<RecipeDto>> Recipes) : PagedResult(Page, PageSize, TotalCount);

    [GroupEndpoint("Recipes")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/", Handler)
                     .WithName("GetRecipes")
                     .WithDescription("Get a paginated list of recipes");
        }
    }

    public static async Task<Ok<HateoasResponse<Response>>> Handler([AsParameters] GetRecipesParameters parameters, [FromServices] IAppDbContext dbContext, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        IQueryable<Recipe> recipesQuery = dbContext.Recipes.AsNoTracking();

        if (parameters.Categories?.Ids.Any() ?? false)
        {
            recipesQuery = recipesQuery.Where(r => r.Categories.Any(c => parameters.Categories.Ids.Contains(c.Value)));
        }

        if (parameters.Ingredients?.Ids.Any() ?? false)
        {
            recipesQuery = recipesQuery.Where(r => r.Ingredients.Any(i => parameters.Ingredients.Ids.Contains(i.IngredientId.Value)));
        }

        int totalCount = await recipesQuery.CountAsync(cancellationToken);

        List<RecipeDto> recipes = await recipesQuery.SetPage(parameters)
                                                    .Select(r => new RecipeDto(r.Id.Value, r.Title, r.ImageURL, (int)r.Difficulty, r.NumberOfServings))
                                                    .ToListAsync(cancellationToken);

        HateoasResponse<Response> response = MapToResponse(recipes, parameters.Page, parameters.PageSize, totalCount, hateoasBuilderFactory);
        return TypedResults.Ok(response);
    }

    private static HateoasResponse<Response> MapToResponse(IEnumerable<RecipeDto> recipes, int page, int pageSize, int totalCount, IHateoasBuilderFactory hateoasBuilderFactory)
    {
        HateoasCollectionResponseBuilder<RecipeDto> recipesBuilder = hateoasBuilderFactory.ForCollection(recipes);

        recipesBuilder.WithCollectionLink()
                      .WithGet(LinkOptions.Create("GetRecipeById", HateoasRelConstants.Self, true), recipe => new { id = recipe.Id });

        Response response = new Response(page, pageSize, totalCount, recipesBuilder.Build().Items);
        HateoasResponseBuilder<Response> responsebuilder = hateoasBuilderFactory.ForItem(response);

        responsebuilder.AddPagedNavigation("GetRecipes", new { page, pageSize });
        responsebuilder.AddPost(LinkOptions.Create("CreateRecipe", HateoasRelConstants.Create, true), null);

        return responsebuilder.Build();
    }
}
