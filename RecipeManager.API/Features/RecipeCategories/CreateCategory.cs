using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.RecipeCategories;

namespace RecipeManager.Api.Features.RecipeCategories;

public static class CreateCategory
{
    public sealed record Request(string Name);

    public sealed record Response(RecipeCategoryId Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .SetValidator(new RecipeCategoryNameValidator())
                .MustAsync(async (name, cancellationToken) =>
                {
                    return await dbContext.RecipeCategories.AnyAsync(category => category.Name == name, cancellationToken);
                }).WithMessage("Recipe category must be unique");
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

    public static async Task<IResult> Handler(Request request, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        RecipeCategory category = RecipeCategory.Create(request.Name);

        dbContext.RecipeCategories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/recipeCategories/{category.Id}", new Response(category.Id));
    }
}
