using RecipeManager.Api.Domain.Recipes.Enums;

namespace RecipeManager.Api.Domain.Recipes.ValueObjects;

public sealed record RecipeSection
{
    public RecipeSectionType Type { get; set; }

    public ICollection<RecipeStep> Steps { get; set; } = [];

    private RecipeSection()
    {
    }

    public static RecipeSection Create(int type, IEnumerable<RecipeStep> steps)
    {
        return new()
        {
            Type = (RecipeSectionType)type,
            Steps = steps.ToList()
        };
    }

    public void UpdateSection(RecipeSection recipeSection)
    {
        IEnumerable<RecipeStep> stepsToRemove = Steps.Where(step => step.Order > recipeSection.Steps.Count);
        Steps = Steps.Except(stepsToRemove).ToList();

        foreach (RecipeStep existingStep in Steps)
        {
            RecipeStep updatedStep = recipeSection.Steps.First(step => step.Order == existingStep.Order);
            existingStep.Description = updatedStep.Description;
            existingStep.ImageUrl = updatedStep.ImageUrl;
        }

        IEnumerable<RecipeStep> stepsToAdd = recipeSection.Steps.Where(step => step.Order > Steps.Count);

        foreach (RecipeStep newStep in stepsToAdd)
        {
            Steps.Add(newStep);
        }
    }
}
