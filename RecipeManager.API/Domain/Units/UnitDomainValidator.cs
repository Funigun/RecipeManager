using RecipeManager.API.Domain.Common.Abstractions;

namespace RecipeManager.API.Domain.Units;

public class UnitDomainValidator : IDomainModelValidator<Unit>
{
    public const int UnitNameMaxLength = 50;
    public const int UnitShortNameMaxLength = 25;

    public void Validate(Unit entity)
    {
        throw new NotImplementedException();
    }
}
