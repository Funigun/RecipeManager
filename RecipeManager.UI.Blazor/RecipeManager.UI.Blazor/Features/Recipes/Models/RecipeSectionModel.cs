namespace RecipeManager.UI.Blazor.Features.Recipes.Models;

public sealed class RecipeSectionModel
{
    public RecipeSectionType SectionType { get; set; }

    public ICollection<RecipeStepModel> Steps { get; set; }

    public bool IsRequired { get; set; }

    public RecipeSectionModel(RecipeSectionType recipeSectionType, ICollection<RecipeStepModel> recipeSteps, bool isRequired)
    {
        SectionType = recipeSectionType;

        Steps = recipeSteps;
        IsRequired = isRequired;
    }

    public string SectionHeader => $"{SectionType.ToString()}{(IsRequired ? "*" : string.Empty)}";

};
