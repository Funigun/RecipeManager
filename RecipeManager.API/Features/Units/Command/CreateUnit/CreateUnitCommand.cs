using FluentValidation;
using FluentValidation.Results;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units.Command.CreateUnit;

public sealed record CreateUnitRequest(string Name, string? ShortName, string PluralName, string? PluralShortName, int Group, bool IsBaseUnit, Guid? PrimaryUnit, int ConversionFactor = 1)
{
    internal Unit ToUnit()
    {
        return Unit.Create
        (
            Name,
            ShortName,
            PluralName,
            PluralShortName,
            (UnitGroup)Group,
            IsBaseUnit,
            PrimaryUnit,
            ConversionFactor
        );
    }
}

public sealed record CreateUnitCommand(CreateUnitRequest Request);

public sealed record CreateUnitResponse(UnitId Id, string Name, string? ShortName, string PluralName, string? PluralShortName, int Group, bool IsBaseUnit, Guid? PrimaryUnit, int ConversionFactor)
{
    internal static CreateUnitResponse FromUnit(Unit unit)
    {
        return new CreateUnitResponse
        (
            unit.Id,
            unit.Name,
            unit.ShortName,
            unit.PluralName,
            unit.PluralShortName,
            (int)unit.Group,
            unit.IsBaseUnit,
            unit.PrimaryUnit?.Value,
            unit.ConversionFactor
        );
    }
}

public sealed class CreateUnitCommandHandler(IAppDbContext dbContext, IValidator<CreateUnitRequest> validator, IAuthorizationPolicy<CreateUnitRequest> authorizationPolicy)
{
    public async Task<Result<CreateUnitResponse>> Handle(CreateUnitCommand command, CancellationToken cancellationToken)
    {
        CreateUnitRequest request = command.Request;

        if (!await authorizationPolicy.IsAuthorized(request))
        {
            return new AuthorizationError();
        }

        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BaseValidator<CreateUnitRequest>.ToValidationError(validationResult);
        }

        Unit unit = request.ToUnit();

        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateUnitResponse.FromUnit(unit);
    }
}
