using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;

namespace RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategoriesForFiltering;

internal sealed record GetRecipeCategoriesForFilteringProjection(Guid Id, int Type, string Name);

public sealed record GetRecipeCategoriesForFilteringCategoryDto(Guid Id, string Name)
{
    internal static GetRecipeCategoriesForFilteringCategoryDto FromProjection(
        GetRecipeCategoriesForFilteringProjection category) => new(category.Id, category.Name);
}

public sealed record GetRecipeCategoriesForFilteringResponse(
    Dictionary<int, IEnumerable<GetRecipeCategoriesForFilteringCategoryDto>> Categories);

public sealed class GetRecipeCategoriesForFilteringQueryHandler(IAppDbContext dbContext)
{
    public async Task<GetRecipeCategoriesForFilteringResponse> Handle(CancellationToken cancellationToken)
    {
        List<GetRecipeCategoriesForFilteringProjection> categories = await dbContext.RecipeCategories
            .AsNoTracking()
            .Select(category => new GetRecipeCategoriesForFilteringProjection(
                category.Id.Value, (int)category.Type, category.Name))
            .ToListAsync(cancellationToken);

        Dictionary<int, IEnumerable<GetRecipeCategoriesForFilteringCategoryDto>> grouped = categories
            .GroupBy(category => category.Type)
            .ToDictionary(
                group => group.Key,
                group => group.Select(GetRecipeCategoriesForFilteringCategoryDto.FromProjection));
        return new GetRecipeCategoriesForFilteringResponse(grouped);
    }
}
