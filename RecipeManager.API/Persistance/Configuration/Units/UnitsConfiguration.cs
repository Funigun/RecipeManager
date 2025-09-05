using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Persistance.Configuration.Units;

public class UnitsConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Unit");

        builder.Property(unit => unit.Id)
               .ValueGeneratedOnAdd();

        builder.Property(unit => unit.Name)
               .HasMaxLength(UnitDomainValidator.UnitNameMaxLength)
               .IsRequired(true);

        builder.Property(unit => unit.ShortName)
               .HasMaxLength(UnitDomainValidator.UnitShortNameMaxLength)
               .IsRequired(false);

        builder.HasOne<Unit>()
               .WithMany()
               .HasForeignKey(unit => unit.PrimaryUnit)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
