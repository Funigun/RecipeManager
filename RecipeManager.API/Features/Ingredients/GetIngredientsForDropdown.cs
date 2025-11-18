using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients;

public static class GetIngredientsForDropdown
{
    public sealed record Response(Guid Id, string Name, string? RecipeUrl);

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

    public static async Task<Results<Ok<IEnumerable<Response>>, NotFound>> Handler([FromQuery] string? ingredientName, [FromServices] IAppDbContext dbContext, [FromServices] ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        IQueryable<Ingredient> query = dbContext.Ingredients.AsNoTracking().Where(ingredient => ingredient.CreatedBy == currentUser.Id);

        if (!string.IsNullOrWhiteSpace(ingredientName))
        {
            ingredientName = ingredientName.Trim().ToLower();
            query = query.Where(ingredient => ingredient.Name.ToLower().Contains(ingredientName));
        }

        IEnumerable<Response> results = await query.OrderBy(ingredient => ingredient.Name)
                                                   .Select(ingredient => new Response(ingredient.Id, ingredient.Name, ingredient.Recipes.Any() ? $"/recipes/{ingredient.Recipes[0]}" : null))
                                                   .ToListAsync(cancellationToken);

        return TypedResults.Ok(results);
    }
}
