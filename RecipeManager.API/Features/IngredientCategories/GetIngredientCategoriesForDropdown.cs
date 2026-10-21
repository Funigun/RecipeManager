using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.IngredientCategories;

public static class GetIngredientCategoriesForDropdown
{
    public sealed record Response(Guid Id, string Name);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>(IngredientCategoryGroup.DropdownRoute, Handler)
                     .WithName("GetIngredientCategoriesDropdown")
                     .WithDescription("Gets all ingredient categories")
                     .WithTags(IngredientCategoryGroup.Tag)
                     .RequireAuthorization("RecipeManagerPolicy")
                     .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }

    internal static async Task<Results<Ok<IEnumerable<Response>>, NotFound>> Handler([FromQuery] string? categoryName, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IEnumerable<Response> results = await dbContext.IngredientCategories
                                                       .Where(category => string.IsNullOrEmpty(categoryName) || category.Name.Contains(categoryName))
                                                       .OrderBy(category => category.Name)
                                                       .Select(category => new Response(category.Id, category.Name))
                                                       .ToListAsync(cancellationToken);
        return TypedResults.Ok(results);
    }
}
