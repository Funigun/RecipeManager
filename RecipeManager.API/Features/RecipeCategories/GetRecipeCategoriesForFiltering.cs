using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.RecipeCategories;

public static class GetRecipeCategoriesForFiltering
{
    public sealed record RecipeCategoryDto(Guid Id, string Name);

    public sealed record Response(Dictionary<int, IEnumerable<RecipeCategoryDto>> Categories);

    [GroupEndpoint("RecipeCategories")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/filtering", Handler)
                     .WithName("GetRecipeCategoriesForFiltering")
                     .WithDescription("Get recipe categories grouped by category type for filtering purposes.");
        }
    }

    internal static async Task<Results<Ok<Response>, NotFound>> Handler(IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        List<RecipeCategory> categories = await dbContext.RecipeCategories.AsNoTracking().ToListAsync(cancellationToken);

        Dictionary<int, IEnumerable<RecipeCategoryDto>> groupedCategories = categories
            .GroupBy(rc => (int)rc.Type)
            .ToDictionary(
                g => g.Key,
                g => g.Select(rc => new RecipeCategoryDto(rc.Id, rc.Name)));

        return TypedResults.Ok(new Response(groupedCategories));
    }
}
