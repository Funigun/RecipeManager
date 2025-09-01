using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Cookbooks;

public sealed class CookbookDomainValidator : IDomainModelValidator<Cookbook>
{
    public const int CookbookTitleMaxLength = 100;

    public const int CookbookDescriptionMaxLength = 1000;

    public void Validate(Cookbook entity)
    {
        throw new NotImplementedException();
    }
}
