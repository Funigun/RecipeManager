using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Persistance.Configuration.Units;

public sealed class UnitIdConverter : ValueConverter<UnitId, Guid>
{
    public UnitIdConverter()
         : base(id => id.Value, value => new UnitId(value))
    {
    }
}
