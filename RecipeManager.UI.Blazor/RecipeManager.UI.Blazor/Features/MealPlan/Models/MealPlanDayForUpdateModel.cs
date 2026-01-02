namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public sealed record class MealPlanDayForUpdateModel(Guid? Id, DateTimeOffset Date, ICollection<Guid> RecipeIds);
