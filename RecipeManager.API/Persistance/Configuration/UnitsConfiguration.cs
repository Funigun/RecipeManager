using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.API.Domain.Units;

namespace RecipeManager.API.Persistance.Configuration;

public class UnitsConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.Property(unit => unit.Id)
               .HasConversion(id => id.Value, value => new UnitId(value))
               .ValueGeneratedOnAdd();

        builder.Property(unit => unit.Name)
               .HasMaxLength(UnitDomainValidator.UnitNameMaxLength)
               .IsRequired(true);

        builder.Property(unit => unit.ShortName)
               .HasMaxLength(UnitDomainValidator.UnitShortNameMaxLength)
               .IsRequired(false);
    }
}
