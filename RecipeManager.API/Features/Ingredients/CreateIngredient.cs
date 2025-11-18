using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Ingredients;

namespace RecipeManager.Api.Features.Ingredients;

public static class CreateIngredient
{
    public sealed record Request(string Name, IEnumerable<Guid> Categories, IEnumerable<Guid> Recipes);

    public sealed record Response(IngredientId Id);

    public sealed class Validator : AbstractValidator<Request>
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
            endpoints.MapStandardValidatedPost<Request, Response>(string.Empty, Handler)
                     .WithName("CreateIngredient")
                     .WithDescription("Creates a new ingredient");
        }
    }

    public static async Task<IResult> Handler([FromBody] Request request, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Ingredient ingredient = request.ToIngredient();

        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/ingredients/{ingredient.Id}", new Response(ingredient.Id));
    }

    private static Ingredient ToIngredient(this Request request)
    {
        return Ingredient.Create
        (
            request.Name,
            request.Categories.Select(c => new IngredientCategoryId(c)),
            request.Recipes.Select(r => new RecipeId(r))
        );
    }
}
