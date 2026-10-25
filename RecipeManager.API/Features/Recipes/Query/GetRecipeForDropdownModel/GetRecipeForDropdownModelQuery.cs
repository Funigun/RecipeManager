using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Recipes.Query.GetRecipeForDropdownModel;

public sealed record GetRecipeForDropdownModelParameters(string RecipeName, int NumberOfRecipesToLoad);
public sealed record GetRecipeForDropdownModelQuery(GetRecipeForDropdownModelParameters Parameters);
internal sealed record GetRecipeForDropdownModelProjection(Guid Id, string Title, string? ImageUrl);
public sealed record GetRecipeForDropdownModelResponse(Guid Id, string Title, string? ImageUrl);

public sealed class GetRecipeForDropdownModelQueryHandler(IAppDbContext dbContext, ICurrentUser currentUser)
{
    public async Task<IEnumerable<GetRecipeForDropdownModelResponse>> Handle(
        GetRecipeForDropdownModelQuery query,
        CancellationToken cancellationToken)
    {
        GetRecipeForDropdownModelParameters parameters = query.Parameters;
        List<Recipe> recipes = await dbContext.Recipes.AsNoTracking()
            .Where(recipe => recipe.Title.Contains(parameters.RecipeName) && recipe.CreatedBy == currentUser.Id)
            .Take(parameters.NumberOfRecipesToLoad)
            .ToListAsync(cancellationToken);
        return recipes.Select(recipe => new GetRecipeForDropdownModelResponse(
            recipe.Id.Value, recipe.Title, recipe.ImageURL));
    }
}
