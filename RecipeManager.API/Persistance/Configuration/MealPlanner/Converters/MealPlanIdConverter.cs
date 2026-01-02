using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManager.Api.Domain.MealPlan;

namespace RecipeManager.Api.Persistance.Configuration.MealPlanner.Converters;

public sealed class MealPlanIdConverter : ValueConverter<MealPlanId, Guid>
{
    public MealPlanIdConverter()
         : base(id => id.Value, value => new MealPlanId(value))
    {
    }
}
