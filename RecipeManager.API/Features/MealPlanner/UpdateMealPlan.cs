using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.MealPlan;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.MealPlanner;

public static class UpdateMealPlan
{
    public sealed record MealPlanDto(Guid? Id, DateTimeOffset Date, IEnumerable<Guid> RecipeIds);

    public sealed record Request(IEnumerable<MealPlanDto> MealPlans);

    public sealed class AuthorizationPolicy(IAppDbContext appDbContext, ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public async Task<bool> IsAuthorized(Request request)
        {
            IEnumerable<Guid> plansToUpdate = request.MealPlans.Where(plan => plan.Id is not null)
                                                               .Select(plan => (Guid)plan.Id!)
                                                               .Distinct();

            int numberOfExistingPlans = await appDbContext.MealPlans.AsNoTracking()
                                                                    .Where(plan => plansToUpdate.Contains(plan.Id) && plan.CreatedBy == currentUser.Id)
                                                                    .CountAsync();

            return plansToUpdate.Count() == numberOfExistingPlans;
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext appDbContext)
        {
            RuleFor(request => request).MustAsync(async (request, cancellationToken) =>
            {
                IEnumerable<Guid> plansToUpdate = request.MealPlans.Where(plan => plan.Id is not null)
                                                                   .Select(plan => (Guid)plan.Id!)
                                                                   .Distinct();

                int numberOfExistingPlans = await appDbContext.MealPlans.AsNoTracking()
                                                                        .Where(plan => plansToUpdate.Contains(plan.Id))
                                                                        .CountAsync(cancellationToken);

                return numberOfExistingPlans == plansToUpdate.Count();
            })
            .WithMessage("Some of meal plans to update do not exist.");

            RuleFor(request => request).MustAsync(async (request, cancellationToken) =>
            {
                IEnumerable<Guid> recipeIds = request.MealPlans.SelectMany(plan => plan.RecipeIds).Distinct();

                int numberOfExistingPlans = await appDbContext.Recipes.AsNoTracking()
                                                                      .Where(plan => recipeIds.Contains(plan.Id))
                                                                      .CountAsync(cancellationToken);

                return numberOfExistingPlans == recipeIds.Count();
            })
            .WithMessage("Some of provided recipes do not exist.");

            RuleFor(request => request).Must(request =>
            {
                return request.MealPlans.Count() == request.MealPlans.Select(plan => plan.Date.Date).Distinct().Count();
            })
            .WithMessage("Meal plan dates must be unique.");
        }
    }

    [GroupEndpoint("MealPlans")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPut<Request, Request>(string.Empty, Handler)
                     .WithName("UpdateMealPlan")
                     .WithDescription("Updates existing meal plans or creates new ones.");
        }
    }

    public static async Task<Results<Ok, BadRequest>> Handler([FromBody] Request request, [FromServices] IAppDbContext appDbContext, [FromServices] ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        IEnumerable<MealPlanDto> plansToCreate = request.MealPlans.Where(plan => plan.Id is null);
        Dictionary<MealPlanId, MealPlanDto> plansToUpdate = request.MealPlans.Where(plan => plan.Id is not null).ToDictionary(plan => new MealPlanId((Guid)plan.Id!), plan => plan);

        foreach (MealPlanDto mealPlanDto in plansToCreate)
        {
            MealPlan newMealPlan = MealPlan.Create(mealPlanDto.Date, mealPlanDto.RecipeIds.Select(id => new Domain.Recipes.RecipeId(id)).ToList());
            await appDbContext.MealPlans.AddAsync(newMealPlan, cancellationToken);
        }

        IEnumerable<MealPlan> existingMealPlans = await appDbContext.MealPlans.Where(plan => plansToUpdate.Keys.Contains(plan.Id))
                                                                              .ToListAsync(cancellationToken);

        foreach (MealPlan mealPlan in existingMealPlans)
        {
            mealPlan.UpdateRecipes(plansToUpdate[mealPlan.Id.Value].RecipeIds.Select(id => new RecipeId(id)).ToList());
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}
