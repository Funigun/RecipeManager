using FluentValidation;
using RecipeManager.Shared.Contracts.Units;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Features.Units.Manage;

public class UnitForManageValidator : BaseAbstractValidator<UnitForManageModel>
{
    public UnitForManageValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new UnitNameValidator());

        RuleFor(x => x.ShortName)
            .SetValidator(new UnitShortNameValidator());

        RuleFor(x => x.PluralName)
            .SetValidator(new UnitPluralNameValidator());

        RuleFor(x => x.PluralShortName)
            .SetValidator(new UnitPluralShortNameValidator());

        When(x => x.PrimaryUnit is not null, () =>
        {
            RuleFor(x => x.ConversionFactor)
                .GreaterThan(1)
                .WithMessage("Conversion Factor must be greater than 1 when Primary Unit is specified");
        });
    }
}
