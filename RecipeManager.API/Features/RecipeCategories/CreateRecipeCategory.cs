using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.RecipeCategories;

namespace RecipeManager.Api.Features.RecipeCategories;

public static class CreateRecipeCategory
{
    public sealed record Request(string Name, int CategoryType);

    public sealed record Response(RecipeCategoryId Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .SetValidator(new RecipeCategoryNameValidator())
                .MustAsync(async (name, cancellationToken) =>
                {
                    return !await dbContext.RecipeCategories.AnyAsync(category => category.Name == name, cancellationToken);
                }).WithMessage("Recipe category must be unique");

            RuleFor(x => x.CategoryType)
                .Must(difficulty => difficulty.IsRecipeCategoryType())
                .WithMessage("Provided invalid recipe category type");
        }
    }

    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public Task<bool> IsAuthorized(Request request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    [GroupEndpoint("RecipeCategories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPost<Request, Response>(string.Empty, Handler)
                     .WithName("CreateRecipeCategory")
                     .WithDescription("Creates a new recipe category with the specified name.");
        }
    }

    public static async Task<IResult> Handler([FromBody] Request request, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        RecipeCategory category = RecipeCategory.Create(request.Name, (RecipeCategoryType)request.CategoryType);

        dbContext.RecipeCategories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/recipeCategories/{category.Id}", new Response(category.Id));
    }
}
