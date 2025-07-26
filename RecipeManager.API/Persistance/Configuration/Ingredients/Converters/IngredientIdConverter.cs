using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManager.Api.Domain.Ingredients;

namespace RecipeManager.Api.Persistance.Configuration.Ingredients.Converters;

public sealed class IngredientIdConverter : ValueConverter<IngredientId, Guid>
{
    public IngredientIdConverter()
         : base(id => id.Value, value => new IngredientId(value))
    {
    }
}
