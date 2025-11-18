using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Shared.Contracts.Authorization;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.RecipeCategories;

public static class GetRecipeCategories
{
    public sealed record Request(int Page = 1, int PageSize = 10, int? CategoryType = null) : PagedParameters(Page, PageSize);

    public sealed record CategoryDto(Guid Id, string Name, string Type);

    public sealed record Response(int Page, int PageSize, int TotalCount, IEnumerable<HateoasResponse<CategoryDto>> Categories) : PagedResult(Page, PageSize, TotalCount);

    [GroupEndpoint("RecipeCategories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<CategoryDto>(string.Empty, Handler)
                     .WithName("GetRecipeCategories")
                     .WithDescription("Gets all recipe categories");
        }
    }

    internal static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler([AsParameters] Request request, [FromServices] ICurrentUser currentUser, [FromServices] IAppDbContext dbContext, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        IQueryable<RecipeCategory> categories = dbContext.RecipeCategories.AsNoTracking();

        if (request.CategoryType is not null)
        {
            categories = categories.Where(category => category.Type == (RecipeCategoryType)request.CategoryType);
        }

        int totalCount = await categories.CountAsync(cancellationToken);

        List<RecipeCategory> results = await categories.OrderBy(category => category.Name)
                                                       .SetPage(request)
                                                       .ToListAsync(cancellationToken);

        HateoasResponse<Response> response = MapToResponse(results, request.Page, request.PageSize, totalCount, currentUser, hateoasBuilderFactory);

        return TypedResults.Ok(response);
    }

    private static HateoasResponse<Response> MapToResponse(IEnumerable<RecipeCategory> categories, int page, int pageSize, int totalCount, ICurrentUser currentUser, IHateoasBuilderFactory hateoasBuilderFactory)
    {
        bool isActionAllowed = currentUser.HasRole(UserRoles.Admin);

        HateoasCollectionResponseBuilder<CategoryDto> categoriesBuilder = hateoasBuilderFactory.ForCollection(categories.Select(ToGetResponse));

        if (isActionAllowed)
        {
            categoriesBuilder
                .WithCollectionLink()
                    .WithDelete(LinkOptions.Create("DeleteRecipeCategory", HateoasRelConstants.Delete, isActionAllowed), unit => new { categoryId = unit.Id })
                .AddPost(LinkOptions.Create("CreateRecipeCategory", HateoasRelConstants.Create, isActionAllowed), null);
        }

        Response response = new(page, pageSize, totalCount, categoriesBuilder.Build().Items);
        HateoasResponseBuilder<Response> responsebuilder = hateoasBuilderFactory.ForItem(response);

        responsebuilder.AddPagedNavigation("GetRecipeCategories", new { page, pageSize });
        return responsebuilder.Build();
    }

    private static CategoryDto ToGetResponse(this RecipeCategory category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Type.ToFriendlyString()
        );
    }
}
