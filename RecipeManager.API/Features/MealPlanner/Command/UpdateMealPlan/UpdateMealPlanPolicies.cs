using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.MealPlanner.Command.UpdateMealPlan;

public sealed class UpdateMealPlanAuthorizationPolicy(IAppDbContext dbContext, ICurrentUser currentUser)
    : IAuthorizationPolicy<UpdateMealPlanRequest>
{
    public async Task<bool> IsAuthorized(UpdateMealPlanRequest request)
    {
        IEnumerable<Guid> plansToUpdate = request.MealPlans
            .Where(plan => plan.Id is not null)
            .Select(plan => (Guid)plan.Id!)
            .Distinct();

        int existingOwnedPlans = await dbContext.MealPlans.AsNoTracking()
            .Where(plan => plansToUpdate.Contains(plan.Id) && plan.CreatedBy == currentUser.Id)
            .CountAsync();

        return plansToUpdate.Count() == existingOwnedPlans;
    }
}

public sealed class UpdateMealPlanValidationPolicy : BaseValidator<UpdateMealPlanRequest>
{
    public UpdateMealPlanValidationPolicy(IAppDbContext dbContext)
    {
        RuleFor(request => request).MustAsync(async (request, cancellationToken) =>
        {
            IEnumerable<Guid> plansToUpdate = request.MealPlans
                .Where(plan => plan.Id is not null)
                .Select(plan => (Guid)plan.Id!)
                .Distinct();
            int existingPlans = await dbContext.MealPlans.AsNoTracking()
                .Where(plan => plansToUpdate.Contains(plan.Id))
                .CountAsync(cancellationToken);
            return existingPlans == plansToUpdate.Count();
        }).WithMessage("Some of meal plans to update do not exist.");

        RuleFor(request => request.MealPlans)
            .MustReferenceExistingRecipes(
                dbContext,
                plans => plans.SelectMany(plan => plan.RecipeIds).Distinct(),
                "Some of provided recipes do not exist.");

        RuleFor(request => request.MealPlans)
            .MustHaveUniqueDates(plan => plan.Date.Date, "Meal plan dates must be unique.");
    }
}
