using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units.Query.GetAllUnits;

internal sealed record GetAllUnitsProjection(
    Guid Id,
    string Name,
    string? ShortName,
    string PluralName,
    string? PluralShortName,
    UnitGroup Group,
    bool IsBaseUnit,
    Guid? PrimaryUnit,
    int ConversionFactor);

public sealed record GetAllUnitsPrimaryUnitDto(Guid Id, string Name, int ConversionFactor);

public sealed record GetAllUnitsResponse(
    Guid UnitId,
    string Name,
    string? ShortName,
    string PluralName,
    string? PluralShortName,
    string Group,
    bool IsBaseUnit,
    GetAllUnitsPrimaryUnitDto? PrimaryUnit,
    ItemActionDto Actions
)
{
    internal static GetAllUnitsResponse FromProjection(GetAllUnitsProjection unit, IReadOnlyDictionary<Guid, GetAllUnitsProjection> units, bool canEdit)
    {
        GetAllUnitsProjection? primaryUnit = unit.PrimaryUnit is Guid primaryUnitId ? units[primaryUnitId] : null;

        return new GetAllUnitsResponse
        (
            unit.Id,
            unit.Name,
            unit.ShortName,
            unit.PluralName,
            unit.PluralShortName,
            unit.Group.ToFriendlyString(),
            unit.IsBaseUnit,
            primaryUnit is not null ? new GetAllUnitsPrimaryUnitDto(primaryUnit.Id, primaryUnit.Name, unit.ConversionFactor) : null,
            new ItemActionDto(true, canEdit, canEdit)
        );
    }
}

public sealed record GetAllUnitsCollectionResponse(CollectionActionDto Actions, IEnumerable<GetAllUnitsResponse> Units);

public sealed class GetAllUnitsQueryHandler(IAppDbContext dbContext, ICurrentUser currentUser)
{
    public async Task<GetAllUnitsCollectionResponse> Handle(CancellationToken cancellationToken)
    {
        List<GetAllUnitsProjection> units = await dbContext.Units.AsNoTracking()
                                                                 .DefaultOrder()
                                                                 .Select(unit => new GetAllUnitsProjection(
                                                                     unit.Id.Value,
                                                                     unit.Name,
                                                                     unit.ShortName,
                                                                     unit.PluralName,
                                                                     unit.PluralShortName,
                                                                     unit.Group,
                                                                     unit.IsBaseUnit,
                                                                     unit.PrimaryUnit == null ? null : unit.PrimaryUnit.Value,
                                                                     unit.ConversionFactor))
                                                                 .ToListAsync(cancellationToken);

        Dictionary<Guid, GetAllUnitsProjection> unitsById = units.ToDictionary(unit => unit.Id);

        bool canEdit = currentUser.HasRole(UserRoles.Admin);
        IEnumerable<GetAllUnitsResponse> responses = units.Select(unit => GetAllUnitsResponse.FromProjection(unit, unitsById, canEdit));

        return new GetAllUnitsCollectionResponse(new CollectionActionDto(canEdit), responses);
    }
}
