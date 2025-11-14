using RecipeManager.Shared.Contracts.Cookbooks;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Cookbooks.Models;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Validators;

public class CookbookModelValidator : BaseAbstractValidator<HateoasResponse<CookbookForManageModel>>
{
    public CookbookModelValidator()
    {
        RuleFor(cookbook => cookbook.Item.Title)
            .SetValidator(new CookbookTitleValidator());

        RuleFor(cookbook => cookbook.Item.Description)
            .SetValidator(new CookbookDescriptionValidator());
    }
}
