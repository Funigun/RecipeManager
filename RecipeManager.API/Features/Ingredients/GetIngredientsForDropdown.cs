using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients;

public static class GetIngredientsForDropdown
{
    public sealed record UnitForDropdownDto(Guid Id, string Name);

    public sealed record Response(Guid Id, string Name, string? RecipeUrl, IEnumerable<UnitForDropdownDto> AvailableUnits);

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/dropdown", Handler)
                     .WithName("GetIngredientsForDropdown")
                     .WithDescription("Gets a list of ingredients for use in dropdowns.");
        }
    }

    public static async Task<Results<Ok<IEnumerable<Response>>, NotFound>> Handler([FromQuery] string? ingredientName, [FromServices] IAppDbContext dbContext, [FromServices] IUnitOfWork unitOfWork, [FromServices] ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        IQueryable<Ingredient> query = dbContext.Ingredients.AsNoTracking()
                                                            .Include(ingredient => ingredient.IngredientUnitConvertions)
                                                            .Where(ingredient => ingredient.CreatedBy == currentUser.Id);

        if (!string.IsNullOrWhiteSpace(ingredientName))
        {
            ingredientName = ingredientName.Trim().ToLower();
            query = query.Where(ingredient => ingredient.Name.ToLower().Contains(ingredientName));
        }

        IEnumerable<Ingredient> ingredients = await query.ToListAsync(cancellationToken);

        IEnumerable<Unit> units = await unitOfWork.Units.GetAllAsync(cancellationToken);

        IEnumerable<Response> results = ingredients.OrderBy(ingredient => ingredient.Name)
                                                   .Select(ingredient => new Response
                                                   (
                                                       ingredient.Id,
                                                       ingredient.Name,
                                                       ingredient.Recipes.Any() ? $"/recipes/{ingredient.Recipes[0]}" : null,
                                                       units.Where(unit => ingredient.GetAvailableUnits().Contains(unit.Id))
                                                            .Select(unit => new UnitForDropdownDto(unit.Id, unit.Name))
                                                   ));

        return TypedResults.Ok(results);
    }
}
