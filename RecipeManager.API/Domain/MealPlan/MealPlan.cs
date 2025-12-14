using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Recipes;

namespace RecipeManager.Api.Domain.MealPlan;

public sealed class MealPlan : AuditableEntity, IEntity<MealPlanId>
{
    private List<RecipeId> _recipes = [];

    public MealPlanId Id { get; set; } = default!;

    public DateTimeOffset Date { get; set; }

    public IReadOnlyList<RecipeId> Recipes => _recipes.ToList();

    private MealPlan()
    {
    }

    public static MealPlan Create(DateTimeOffset day, List<RecipeId> recipes)
    {
        return new()
        {
            Date = day,
            _recipes = recipes,
        };
    }
}
