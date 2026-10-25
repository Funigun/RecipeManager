using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units.Query.GetUnitById;

public sealed record GetUnitByIdQuery(Guid UnitId);

internal sealed record GetUnitByIdProjection(
    Guid Id,
    string Name,
    string? ShortName,
    string PluralName,
    string? PluralShortName,
    int Group,
    bool IsBaseUnit,
    Guid? PrimaryUnit,
    int ConversionFactor);

public sealed record GetUnitByIdResponse(
    Guid UnitId,
    string Name,
    string? ShortName,
    string PluralName,
    string? PluralShortName,
    int Group,
    bool IsBaseUnit,
    Guid? PrimaryUnit,
    int ConversionFactor,
    ItemActionDto Actions
)
{
    internal static GetUnitByIdResponse FromProjection(GetUnitByIdProjection unit, bool canEdit)
    {
        return new GetUnitByIdResponse
        (
            unit.Id,
            unit.Name,
            unit.ShortName,
            unit.PluralName,
            unit.PluralShortName,
            unit.Group,
            unit.IsBaseUnit,
            unit.PrimaryUnit,
            unit.ConversionFactor,
            new ItemActionDto(true, canEdit, canEdit)
        );
    }
}

public sealed class GetUnitByIdQueryHandler(IAppDbContext dbContext, ICurrentUser currentUser)
{
    public async Task<Result<GetUnitByIdResponse>> Handle(GetUnitByIdQuery query, CancellationToken cancellationToken)
    {
        UnitId id = new(query.UnitId);
        GetUnitByIdProjection? unit = await dbContext.Units.AsNoTracking()
            .Where(unit => unit.Id == id)
            .Select(unit => new GetUnitByIdProjection(
                unit.Id.Value,
                unit.Name,
                unit.ShortName,
                unit.PluralName,
                unit.PluralShortName,
                (int)unit.Group,
                unit.IsBaseUnit,
                unit.PrimaryUnit == null ? null : unit.PrimaryUnit.Value,
                unit.ConversionFactor))
            .FirstOrDefaultAsync(cancellationToken);

        if (unit is null)
        {
            return new NotFoundError();
        }

        bool canEdit = currentUser.HasRole(UserRoles.Admin);
        return GetUnitByIdResponse.FromProjection(unit, canEdit);
    }
}
