using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Ingredients;

namespace RecipeManager.Api.Features.Ingredients;

public static class UpdateIngredient
{
    public record struct Request(Guid Value) : IRequestId<Request>
    {
    }

    public sealed record IngredientDto(string Name, IEnumerable<Guid> Categories, IEnumerable<Guid> Recipes);

    public sealed class AuthorizationPolicy(IAppDbContext appDbContext, ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            return await appDbContext.Ingredients.AsNoTracking()
                                                 .AnyAsync(ingredient => ingredient.Id == new IngredientId(request.Value) &&
                                                           ingredient.CreatedBy == currentUser.Id);
        }
    }

    public sealed class Validator : AbstractValidator<IngredientDto>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Name).SetValidator(new IngredientNameValidator());
            When(request => request.Categories.Any(), () =>
            {
                RuleFor(x => x.Categories)
                    .MustAsync(async (categories, cancellationToken) =>
                    {
                        IEnumerable<IngredientCategoryId> categoryIds = categories.Select(c => new IngredientCategoryId(c));
                        return await dbContext.IngredientCategories.AsNoTracking()
                                                                   .Where(category => categoryIds.Contains(category.Id))
                                                                   .CountAsync(cancellationToken) == categories.Count();
                    }).WithMessage("Some of categories does not exist");
            });
            When(request => request.Recipes.Any(), () =>
            {
                RuleFor(x => x.Recipes)
                    .MustAsync(async (recipes, cancellationToken) =>
                    {
                        IEnumerable<RecipeId> recipeIds = recipes.Select(r => new RecipeId(r));
                        return await dbContext.Recipes.AsNoTracking()
                                                      .Where(recipe => recipeIds.Contains(recipe.Id))
                                                      .CountAsync(cancellationToken) == recipes.Count();
                    }).WithMessage("Some of recipes does not exist");
            });
        }
    }

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPut<IngredientDto>("/{ingredientId}", Handler)
                     .WithName("UpdateIngredient")
                     .WithDescription("Updates an existing ingredient");
        }
    }

    public static async Task<IResult> Handler(Guid ingredientId, IngredientDto request, IAppDbContext dbContext, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        IngredientId id = new(ingredientId);

        Ingredient? ingredient = await dbContext.Ingredients.FirstOrDefaultAsync(i => i.Id == id && i.CreatedBy == currentUser.Id, cancellationToken)
                              ?? throw new EntityNotFoundException<Ingredient, IngredientId>(id);

        ingredient.Update(request.Name, request.Categories.Select(c => new IngredientCategoryId(c)), request.Recipes.Select(r => new RecipeId(r)));

        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}
