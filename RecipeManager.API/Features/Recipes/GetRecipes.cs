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
    public sealed record GetRecipesParameters(int PageNumber = 1, int PageSize = 10) : PagedParameters(PageNumber, PageSize);

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

        int totalCount = await recipesQuery.CountAsync(cancellationToken);

        List<RecipeDto> recipes = await recipesQuery.SetPage(parameters)
                                                    .Select(r => new RecipeDto(r.Id.Value, r.Title, r.ImageURL, (int)r.Difficulty, r.NumberOfServings))
                                                    .ToListAsync(cancellationToken);

        HateoasResponse<Response> response = MapToResponse(recipes, parameters.PageNumber, parameters.PageSize, totalCount, hateoasBuilderFactory);
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

        return responsebuilder.Build();
    }
}
