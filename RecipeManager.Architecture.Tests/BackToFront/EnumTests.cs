using System.Reflection;

namespace RecipeManager.Architecture.Tests.BackToFront;

public sealed class EnumTests
{
    private readonly Assembly _coreAssembly = typeof(Api.Domain.Recipes.Enums.RecipeCategoryType).Assembly;
    private readonly Assembly _blazorAssembly = typeof(UI.Blazor.Services.Authentication.AuthenticationService).Assembly;

    [Fact]
    public void EachDomainEnum_ShouldHaveCorrespondingEnumInBlazorFrontend()
    {
        // Arrange
        IEnumerable<Type>? domainEnumTypes = _coreAssembly.GetTypes()
                                                          .Where(t => t.IsEnum && t.Namespace != null && t.Namespace.Contains("Domain."));

        IEnumerable<Type>? blazorEnumTypes = _blazorAssembly.GetTypes()
                                                            .Where(t => t.IsEnum && t.Namespace != null && t.Namespace.Contains("UI.Blazor.Features"));

        foreach (Type domainEnumType in domainEnumTypes)
        {
            // Act
            string? sharedEnumTypeName = domainEnumType.Name;
            Type? sharedEnumType = blazorEnumTypes.FirstOrDefault(t => t.Name == sharedEnumTypeName);

            // Verify enum existance
            Assert.NotNull(sharedEnumType);
            Assert.True(sharedEnumType!.IsEnum, $"Corresponding frontend enum for {domainEnumType.Name} not found or is not an enum.");

            // Compare enum by member names
            string[] domainNames = Enum.GetNames(domainEnumType);
            string[] sharedNames = Enum.GetNames(sharedEnumType);
            Array domainValues = Enum.GetValues(domainEnumType);
            Array sharedValues = Enum.GetValues(sharedEnumType);

            Assert.True(domainNames.Length == sharedNames.Length, $"Domain enum {domainEnumType.Name} and frontend enum {sharedEnumType.Name} do not have the same number of members.");

            for (int i = 0; i < domainNames.Length; i++)
            {
                Assert.True(domainNames[i] == sharedNames[i], $"Domain enum  member ({domainEnumType.Name} - {domainNames[i]}) and frontend enum member ({sharedEnumType.Name} - {sharedNames[i]}) do not match.");
                Assert.True((int)domainValues.GetValue(i)! == (int)sharedValues.GetValue(i)!, $"Domain enum value ({domainEnumType.Name} - {domainValues.GetValue(i)}) and frontend enum value ({sharedEnumType.Name} - {sharedValues.GetValue(i)}) do not match.");
            }
        }
    }
}
