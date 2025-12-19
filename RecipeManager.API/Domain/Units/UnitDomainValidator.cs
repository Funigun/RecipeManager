using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Units;

public class UnitDomainValidator : IDomainModelValidator<Unit>
{
    public const int UnitNameMaxLength = 50;
    public const int UnitShortNameMaxLength = 25;
    public const int UnitPluralNameMaxLength = 55;
    public const int UnitPluralShortNameMaxLength = 30;

    public void Validate(Unit entity)
    {
        throw new NotImplementedException();
    }
}
