using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;

namespace RecipeManager.Api.Features.IngredientCategories.Query.GetIngredientCategoriesForDropdown;

public sealed record GetIngredientCategoriesForDropdownRequest(string? CategoryName);

public sealed record GetIngredientCategoriesForDropdownQuery(GetIngredientCategoriesForDropdownRequest Request);

internal sealed record GetIngredientCategoriesForDropdownProjection(Guid Id, string Name);

public sealed record GetIngredientCategoriesForDropdownResponse(Guid Id, string Name)
{
    internal static GetIngredientCategoriesForDropdownResponse FromProjection(
        GetIngredientCategoriesForDropdownProjection category) => new(category.Id, category.Name);
}

public sealed class GetIngredientCategoriesForDropdownQueryHandler(IAppDbContext dbContext)
{
    public async Task<IEnumerable<GetIngredientCategoriesForDropdownResponse>> Handle(
        GetIngredientCategoriesForDropdownQuery query,
        CancellationToken cancellationToken)
    {
        string? categoryName = query.Request.CategoryName;
        List<GetIngredientCategoriesForDropdownProjection> categories = await dbContext.IngredientCategories
            .AsNoTracking()
            .Where(category => string.IsNullOrEmpty(categoryName) || category.Name.Contains(categoryName))
            .OrderBy(category => category.Name)
            .Select(category => new GetIngredientCategoriesForDropdownProjection(category.Id.Value, category.Name))
            .ToListAsync(cancellationToken);

        return categories.Select(GetIngredientCategoriesForDropdownResponse.FromProjection);
    }
}
