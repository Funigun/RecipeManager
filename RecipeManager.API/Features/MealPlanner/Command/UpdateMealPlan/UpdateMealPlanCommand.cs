using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.MealPlan;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.MealPlanner.Command.UpdateMealPlan;

public sealed record UpdateMealPlanDto(Guid? Id, DateTimeOffset Date, IEnumerable<Guid> RecipeIds);

public sealed record UpdateMealPlanRequest(IEnumerable<UpdateMealPlanDto> MealPlans);

public sealed record UpdateMealPlanCommand(UpdateMealPlanRequest Request);

public sealed class UpdateMealPlanCommandHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IAuthorizationPolicy<UpdateMealPlanRequest> authorizationPolicy,
    IValidator<UpdateMealPlanRequest> validator)
{
    public async Task<Result<bool>> Handle(UpdateMealPlanCommand command, CancellationToken cancellationToken)
    {
        UpdateMealPlanRequest request = command.Request;
        if (!await authorizationPolicy.IsAuthorized(request))
        {
            return new AuthorizationError();
        }

        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BaseValidator<UpdateMealPlanRequest>.ToValidationError(validationResult);
        }

        IEnumerable<UpdateMealPlanDto> plansToCreate = request.MealPlans.Where(plan => plan.Id is null);
        Dictionary<MealPlanId, UpdateMealPlanDto> plansToUpdate = request.MealPlans
            .Where(plan => plan.Id is not null)
            .ToDictionary(plan => new MealPlanId((Guid)plan.Id!), plan => plan);

        foreach (UpdateMealPlanDto mealPlanDto in plansToCreate)
        {
            MealPlan mealPlan = MealPlan.Create(
                mealPlanDto.Date,
                mealPlanDto.RecipeIds.Select(id => new RecipeId(id)).ToList());
            await dbContext.MealPlans.AddAsync(mealPlan, cancellationToken);
        }

        IEnumerable<MealPlan> existingMealPlans = await dbContext.MealPlans
            .Where(plan => plansToUpdate.Keys.Contains(plan.Id) && plan.CreatedBy == currentUser.Id)
            .ToListAsync(cancellationToken);
        foreach (MealPlan mealPlan in existingMealPlans)
        {
            mealPlan.UpdateRecipes(plansToUpdate[mealPlan.Id].RecipeIds.Select(id => new RecipeId(id)).ToList());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
