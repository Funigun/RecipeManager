using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.RecipeCategories;

public static class GetRecipeCategoriesForDropdown
{
    public sealed record Response(Guid Id, string Name);

    [GroupEndpoint("RecipeCategories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/dropdown", Handler)
                     .WithName("GetRecipeCategoriesDropdown")
                     .WithDescription("Gets all recipe categories");
        }
    }

    internal static async Task<Results<Ok<IEnumerable<Response>>, NotFound>> Handler([FromQuery] string? categoryName, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IEnumerable<Response> results = await dbContext.RecipeCategories
                                                       .Where(category => string.IsNullOrEmpty(categoryName) || category.Name.Contains(categoryName))
                                                       .OrderBy(category => category.Name)
                                                       .Select(category => new Response(category.Id, category.Name))
                                                       .ToListAsync(cancellationToken);
        return TypedResults.Ok(results);
    }
}
