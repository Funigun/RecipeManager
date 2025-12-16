namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public sealed record MealPlanForUpdateModel(Guid? Id, DateTimeOffset Date, ICollection<Guid> RecipeIds);
