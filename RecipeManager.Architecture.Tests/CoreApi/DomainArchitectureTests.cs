using System.Reflection;
using NetArchTest.Rules;
using RecipeManager.Api.Domain.Common.Abstractions;
using TestResult = NetArchTest.Rules.TestResult;

namespace RecipeManager.Architecture.Tests.CoreApi;

[Trait("Core.Api", "Domain")]
public sealed class DomainArchitectureTests
{
    private readonly Assembly _assembly = typeof(IEntity).Assembly;

    [Fact]
    public void All_Entities_Should_Have_Corresponding_DomainValidator()
    {
        IEnumerable<Type> entityTypes = Types.InAssembly(_assembly)
                                             .That()
                                             .ImplementInterface(typeof(IEntity<>))
                                             .GetTypes();

        IEnumerable<Type> validatorTypes = Types.InAssembly(_assembly)
                                                .That()
                                                .ImplementInterface(typeof(IDomainModelValidator<>))
                                                .GetTypes();

        List<Type>? missingValidators = entityTypes.Where(entity =>
        {
            return !validatorTypes.Any(validator => validator.GetInterfaces().Any(type =>
            {
                return type.IsGenericType &&
                       type.GetGenericTypeDefinition() == typeof(IDomainModelValidator<>) &&
                       type.GenericTypeArguments[0] == entity;
            }));
        }).ToList();

        Assert.True(
            missingValidators.Count == 0,
            $"Missing IDomainValidator implementations for: {string.Join(", ", missingValidators.Select(t => t.Name))}");
    }

    [Fact]
    public void Domain_Should_Not_Have_Dependencies_On_Other_Layers()
    {
        string[] namespacesToCheck = ["RecipeManager.API.Application", "RecipeManager.API.Persistence",
                                             "RecipeManager.API.Presentation", "RecipeManager.API.Features"];

        TestResult? result = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace("RecipeManager.API.Domain")
            .ShouldNot()
            .HaveDependencyOnAny(namespacesToCheck)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            $"Domain layer has invalid dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
