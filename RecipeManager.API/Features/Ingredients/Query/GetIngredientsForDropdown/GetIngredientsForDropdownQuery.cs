using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Ingredients.Query.GetIngredientsForDropdown;

public sealed record GetIngredientsForDropdownQuery(string? IngredientName);

public sealed record GetIngredientsForDropdownUnitDto(Guid Id, string Name);

public sealed record GetIngredientsForDropdownResponse(
    Guid Id,
    string Name,
    string? RecipeUrl,
    IEnumerable<GetIngredientsForDropdownUnitDto> AvailableUnits);

public sealed class GetIngredientsForDropdownQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser)
{
    private sealed record UnitInfo(UnitId Id, string Name);

    public async Task<IEnumerable<GetIngredientsForDropdownResponse>> Handle(
        GetIngredientsForDropdownQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Ingredient> ingredientsQuery = dbContext.Ingredients.AsNoTracking()
            .Include(ingredient => ingredient.IngredientUnitConvertions)
            .Where(ingredient => ingredient.CreatedBy == currentUser.Id);

        string? ingredientName = query.IngredientName;
        if (!string.IsNullOrWhiteSpace(ingredientName))
        {
            ingredientName = ingredientName.Trim().ToLower();
            ingredientsQuery = ingredientsQuery.Where(ingredient => ingredient.Name.ToLower().Contains(ingredientName));
        }

        IEnumerable<Ingredient> ingredients = await ingredientsQuery.ToListAsync(cancellationToken);
        List<UnitInfo> units = await dbContext.Units.AsNoTracking()
            .Select(unit => new UnitInfo(unit.Id, unit.Name))
            .ToListAsync(cancellationToken);

        return ingredients.OrderBy(ingredient => ingredient.Name)
            .Select(ingredient => new GetIngredientsForDropdownResponse(
                ingredient.Id,
                ingredient.Name,
                ingredient.Recipes.Any() ? $"/recipes/{ingredient.Recipes[0]}" : null,
                units.Where(unit => ingredient.GetAvailableUnits().Contains(unit.Id))
                    .Select(unit => new GetIngredientsForDropdownUnitDto(unit.Id.Value, unit.Name))));
    }
}
