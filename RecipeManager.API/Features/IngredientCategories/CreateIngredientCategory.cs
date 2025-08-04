using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.IngredientCategories;

namespace RecipeManager.Api.Features.IngredientCategories;

public static class CreateIngredientCategory
{
    public sealed record Request(string Name);

    public sealed record Response(IngredientCategoryId Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .SetValidator(new IngredientCategoryNameValidator())
                .MustAsync(async (name, cancellationToken) =>
                {
                    return !await dbContext.IngredientCategories.AnyAsync(category => category.Name == name, cancellationToken);
                }).WithMessage("Ingredient category must be unique");
        }
    }

    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public Task<bool> IsAuthorized(Request request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPost<Request, Response>(string.Empty, Handler)
                     .WithName("CreateIngredientCategory")
                     .WithDescription("Creates a new ingredient category with the specified name.");
        }
    }

    public static async Task<IResult> Handler([FromBody] Request request, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IngredientCategory category = IngredientCategory.Create(request.Name);

        dbContext.IngredientCategories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/ingredientCategories/{category.Id}", new Response(category.Id));
    }
}
