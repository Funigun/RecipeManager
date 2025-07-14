using RecipeManager.Shared.Contracts.Units;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Features.Units.CreateUnit;

public class UnitForCreateValidator : BaseAbstractValidator<UnitForCreateModel>
{
    public UnitForCreateValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new UnitNameValidator());

        RuleFor(x => x.ShortName)
            .SetValidator(new UnitShortNameValidator());
    }
}
