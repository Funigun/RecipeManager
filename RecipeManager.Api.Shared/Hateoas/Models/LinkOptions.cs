namespace RecipeManager.Api.Shared.Hateoas.Models;

public sealed class LinkOptions
{
    required public string Endpoint { get; set; }

    required public string Rel { get; set; }

    required public bool IsActionAllowed { get; set; }

    private LinkOptions() { }

    public static LinkOptions Create(string endpoint, string rel, bool isActionAllowed)
    {
        return new()
        { 
            Endpoint = endpoint,
            Rel = rel,
            IsActionAllowed = isActionAllowed,
        };
    }
}
