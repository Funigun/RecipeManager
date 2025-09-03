using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Cookbooks;

namespace RecipeManager.Api.Persistance.Configuration.Cookbooks;

public sealed class CookbookConfiguration : IEntityTypeConfiguration<Cookbook>
{
    public void Configure(EntityTypeBuilder<Cookbook> builder)
    {
        builder.ToTable("Cookbook");

        builder.Property(cookbook => cookbook.Id)
               .ValueGeneratedOnAdd();

        builder.Property(cookbook => cookbook.Title)
               .HasMaxLength(CookbookDomainValidator.CookbookTitleMaxLength)
               .IsRequired(true);

        builder.Property(cookbook => cookbook.Description)
               .HasMaxLength(CookbookDomainValidator.CookbookDescriptionMaxLength)
               .IsRequired(false);

        builder.HasMany(cookbook => cookbook.Categories)
               .WithOne(category => category.Cookbook);
    }
}
