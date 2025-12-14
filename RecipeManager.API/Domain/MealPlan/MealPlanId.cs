namespace RecipeManager.Api.Domain.MealPlan;

public record MealPlanId(Guid Value)
{
    public static implicit operator Guid(MealPlanId id) => id.Value;

    public static implicit operator MealPlanId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
