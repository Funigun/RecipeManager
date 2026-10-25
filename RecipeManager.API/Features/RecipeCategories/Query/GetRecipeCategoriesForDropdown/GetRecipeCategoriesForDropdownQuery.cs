using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;

namespace RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategoriesForDropdown;

public sealed record GetRecipeCategoriesForDropdownRequest(string? CategoryName);

public sealed record GetRecipeCategoriesForDropdownQuery(GetRecipeCategoriesForDropdownRequest Request);

internal sealed record GetRecipeCategoriesForDropdownProjection(Guid Id, string Name);

public sealed record GetRecipeCategoriesForDropdownResponse(Guid Id, string Name)
{
    internal static GetRecipeCategoriesForDropdownResponse FromProjection(
        GetRecipeCategoriesForDropdownProjection category) => new(category.Id, category.Name);
}

public sealed class GetRecipeCategoriesForDropdownQueryHandler(IAppDbContext dbContext)
{
    public async Task<IEnumerable<GetRecipeCategoriesForDropdownResponse>> Handle(
        GetRecipeCategoriesForDropdownQuery query,
        CancellationToken cancellationToken)
    {
        string? categoryName = query.Request.CategoryName;
        List<GetRecipeCategoriesForDropdownProjection> categories = await dbContext.RecipeCategories
            .AsNoTracking()
            .Where(category => string.IsNullOrEmpty(categoryName) || category.Name.Contains(categoryName))
            .OrderBy(category => category.Name)
            .Select(category => new GetRecipeCategoriesForDropdownProjection(category.Id.Value, category.Name))
            .ToListAsync(cancellationToken);
        return categories.Select(GetRecipeCategoriesForDropdownResponse.FromProjection);
    }
}
