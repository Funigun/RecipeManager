using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.Units;

namespace RecipeManager.Api.Features.Units;

public static class UpdateUnit
{
    public sealed record Request(string Name, string? ShortName, int Group);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext, IHttpContextAccessor context)
        {
            RuleFor(x => x.Name)
                .SetValidator(new UnitNameValidator())
                .MustAsync(async (name, cancellationToken) =>
                {
                    string id = context.HttpContext!.GetRouteData().Values["unitId"]!.ToString()!;
                    UnitId unitId = new(Guid.Parse(id));
                    return !await dbContext.Units.AnyAsync(unit => unit.Name == name && unit.Id != unitId, cancellationToken);
                }).WithMessage("Unit Name must be unique");

            When(x => x.ShortName is not null, () =>
            {
                RuleFor(x => x.ShortName)
                    .SetValidator(new UnitShortNameValidator())
                    .MustAsync(async (shortName, cancellationToken) =>
                    {
                        string id = context.HttpContext!.GetRouteData().Values["unitId"]!.ToString()!;
                        UnitId unitId = new(Guid.Parse(id));
                        return !await dbContext.Units.AnyAsync(unit => unit.ShortName == shortName && unit.Id != unitId, cancellationToken);
                    }).WithMessage("Unit Short Name must be unique");
            });

            RuleFor(x => x.Group)
                .Must(group => group.IsUnitGroup())
                    .WithMessage("Invalid unit group")
                    .WithName("Unit Group");
        }
    }

    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public Task<bool> IsAuthorized(Request request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    [GroupEndpoint("Units")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPut<Request, Request>("/{unitId}", Handler)
                     .WithName("UpdateMeasurementUnit")
                     .WithDescription("Updates a measurement unit");
        }
    }

    internal static async Task<Results<NoContent, NotFound, BadRequest>> Handler(Guid unitId, Request request, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        UnitId id = new(unitId);
        Unit? unit = await dbContext.Units.FindAsync([id], cancellationToken) ?? throw new EntityNotFoundException<Unit, UnitId>(id);

        unit.Name = request.Name;
        unit.ShortName = request.ShortName;
        unit.Group = (UnitGroup)request.Group;

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
