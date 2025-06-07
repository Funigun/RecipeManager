namespace RecipeManager.Api.Shared.Endpoint;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]

public class GroupEndpointAttribute(string groupName) : Attribute
{
    public string GroupName { get; } = groupName;
}
