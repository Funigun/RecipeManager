using RecipeManager.Shared.Contracts.Units;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Features.Units.UpdateUnit;

public class UnitForUpdateValidator : BaseAbstractValidator<UnitForUpdateModel>
{
    public UnitForUpdateValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new UnitNameValidator());

        RuleFor(x => x.ShortName)
            .SetValidator(new UnitShortNameValidator());
    }
}
