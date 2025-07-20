using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManager.Api.Domain.Ingredients;

namespace RecipeManager.Api.Persistance.Configuration.Ingredients.Converters;

public sealed class IngredientCategoryIdConverter : ValueConverter<IngredientCategoryId, Guid>
{
    public IngredientCategoryIdConverter()
         : base(id => id.Value, value => new IngredientCategoryId(value))
    {
    }
}
