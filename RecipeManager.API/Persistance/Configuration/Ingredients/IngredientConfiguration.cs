using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Ingredients;

namespace RecipeManager.Api.Persistance.Configuration.Ingredients;

public sealed class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredient");

        builder.Property(ingredient => ingredient.Id)
               .ValueGeneratedOnAdd();

        builder.Property(ingredient => ingredient.Name)
               .HasMaxLength(IngredientDomainValidator.IngredientNameMaxLength)
               .IsRequired(true);

        builder.ComplexProperty(ingredient => ingredient.NutritionalValue, nutritionalValue =>
        {
            nutritionalValue.Property(c => c.Calories)
                            .IsRequired(true);

            nutritionalValue.Property(c => c.Carbohydrates)
                            .IsRequired(true);

            nutritionalValue.Property(c => c.Fats)
                            .IsRequired(true);

            nutritionalValue.Property(c => c.Proteins)
                            .IsRequired(true);

            nutritionalValue.Property(c => c.IngredientAmount)
                            .IsRequired(true);

            nutritionalValue.Property(c => c.IngredientUnit)
                            .IsRequired(true);

            nutritionalValue.ToJson();
        });

        builder.HasOne<IngredientCategory>()
               .WithMany()
               .HasForeignKey(ingredient => ingredient.ShoppingListCategoryId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.ClientSetNull);

        builder.OwnsMany(ingredient => ingredient.Categories, categories =>
        {
            categories.ToTable("IngredientToIngredientCategory");
        });

        builder.OwnsMany(ingredient => ingredient.Recipes, recipes =>
        {
            recipes.ToTable("IngredientToRecipe");
        });
    }
}
