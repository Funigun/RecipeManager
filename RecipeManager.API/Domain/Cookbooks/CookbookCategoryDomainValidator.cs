using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Cookbooks;

public sealed class CookbookCategoryDomainValidator : IDomainModelValidator<CookbookCategory>
{
    public const int CookbookCategoryNameMaxLength = 100;

    public void Validate(CookbookCategory entity)
    {
        throw new NotImplementedException();
    }
}
