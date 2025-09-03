using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Cookbooks;

public static class DeleteCookbook
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed class AuthorizationPolicy(ICurrentUser currentUser, IAppDbContext dbContext) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            return await dbContext.Cookbooks.AnyAsync(cookbook => cookbook.Id == new CookbookId(request.Id) && cookbook.CreatedBy == currentUser.Id);
        }
    }

    [GroupEndpoint("Cookbooks")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Request>("/{cookbookId}", Handler)
                     .WithName("DeleteCookbook")
                     .WithDescription("Deletes a cookbook");
        }
    }

    public static async Task<IResult> Handler(Request cookbookId, [FromServices] IAppDbContext dbContext, [FromServices] ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        CookbookId id = new(cookbookId.Id);
        Cookbook? cookbook = await dbContext.Cookbooks.FirstOrDefaultAsync(cookbook => cookbook.Id == id && cookbook.CreatedBy == currentUser.Id, cancellationToken)
                          ?? throw new EntityNotFoundException<Cookbook, CookbookId>(id);

        dbContext.Cookbooks.Remove(cookbook);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
