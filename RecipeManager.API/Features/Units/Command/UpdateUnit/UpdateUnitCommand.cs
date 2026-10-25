using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units.Command.UpdateUnit;

public sealed record UpdateUnitRequest(
    string Name,
    string? ShortName,
    string PluralName,
    string? PluralShortName,
    int Group,
    bool IsBaseUnit,
    Guid? PrimaryUnit,
    int ConversionFactor)
{
    internal void ApplyTo(Unit unit)
    {
        unit.Update(
            Name,
            ShortName,
            PluralName,
            PluralShortName,
            (UnitGroup)Group,
            IsBaseUnit,
            PrimaryUnit,
            ConversionFactor);
    }
}

public sealed record UpdateUnitCommand(Guid UnitId, UpdateUnitRequest Request);

public sealed class UpdateUnitCommandHandler(IAppDbContext dbContext, IValidator<UpdateUnitRequest> validator, IAuthorizationPolicy<UpdateUnitRequest> authorizationPolicy)
{
    public async Task<Result<UnitId>> Handle(UpdateUnitCommand command, CancellationToken cancellationToken)
    {
        UpdateUnitRequest request = command.Request;

        if (!await authorizationPolicy.IsAuthorized(request))
        {
            return new AuthorizationError();
        }

        UnitId id = new(command.UnitId);
        Unit? unit = await dbContext.Units.FirstOrDefaultAsync(unit => unit.Id == id, cancellationToken);
        if (unit is null)
        {
            return new NotFoundError();
        }

        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<UpdateUnitRequest>.ToValidationError(validationResult);
        }

        request.ApplyTo(unit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return unit.Id;
    }
}
