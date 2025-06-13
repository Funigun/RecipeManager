using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using Moq;
using RecipeManager.Identity.API.Domain;
using RecipeManager.Identity.API.Features.Account;

namespace RecipeManager.Tests.UnitTests.Account;

[Trait("Account", TestCategories.UnitTests)]
public class UserRegistrationTests
{
    [Fact]
    public async Task Validator_ShouldHaveError_WhenUserNameIsEmpty()
    {
        // Arrange
        IQueryable<User> users = new List<User>().AsQueryable();

        Mock<IUserStore<User>> userStoreMock = new();
        Mock<UserManager<User>> userManagerMock = new(userStoreMock.Object, null, null, null, null, null, null, null, null);

        userManagerMock.Setup(um => um.Users).Returns(users);

        UserRegistration.Validator validator = new(userManagerMock.Object);
        UserRegistration.Request request = new(new() { UserName = "", Password = "Test", Email = "Test" });

        // Act
        TestValidationResult<UserRegistration.Request> result = await validator.TestValidateAsync(request, null, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(req => req.UserDto.UserName);
    }

    [Fact]
    public async Task Validator_ShouldHaveError_WhenUserNameIsTooLong()
    {
        // Arrange
        const int numberOfUserNameCharacters = 16;

        IQueryable<User> users = new List<User>().AsQueryable();

        Mock<IUserStore<User>> userStoreMock = new();
        Mock<UserManager<User>> userManagerMock = new(userStoreMock.Object, null, null, null, null, null, null, null, null);

        userManagerMock.Setup(um => um.Users).Returns(users);

        UserRegistration.Validator validator = new(userManagerMock.Object);

        UserRegistration.Request request = new(new() { UserName = new('t', numberOfUserNameCharacters), Password = "Test", Email = "Test" });
        
        // Act
        TestValidationResult<UserRegistration.Request> result = await validator.TestValidateAsync(request, null, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(req => req.UserDto.UserName);
    }

    [Fact]
    public async Task Validator_ShouldHaveError_WhenUserNameIsDuplicated()
    {
        // Arrange
        User existingUser = new() { UserName = "duplicateUser" };
        IQueryable<User> users = new[] { existingUser }.AsQueryable();

        Mock<IUserStore<User>> userStoreMock = new ();
        Mock<UserManager<User>> userManagerMock = new (userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        userManagerMock.Setup(um => um.FindByNameAsync("duplicateUser")).Returns(Task.FromResult(existingUser));

        UserRegistration.Validator validator = new(userManagerMock.Object);
        UserRegistration.Request request = new(new() { UserName = "duplicateUser", Password = "ValidTestPass123!", Email = "testEmail@gmail.com" });
        // Act
        
        TestValidationResult<UserRegistration.Request> result = await validator.TestValidateAsync(request, null, TestContext.Current.CancellationToken);
        
        // Assert
        result.ShouldHaveValidationErrorFor(req => req.UserDto.UserName)
              .WithErrorMessage("User Name is already in use");
    }

    [Fact]
    public async Task Validator_ShouldHaveError_WhenEmailIsEmpty()
    {
        // Arrange
        IQueryable<User> users = new List<User>().AsQueryable();

        Mock<IUserStore<User>> userStoreMock = new();
        Mock<UserManager<User>> userManagerMock = new(userStoreMock.Object, null, null, null, null, null, null, null, null);

        userManagerMock.Setup(um => um.Users).Returns(users);

        UserRegistration.Validator validator = new(userManagerMock.Object);

        UserRegistration.Request request = new(new() { UserName = "Test", Password = "Test", Email = "" });

        // Act
        TestValidationResult<UserRegistration.Request> result = await validator.TestValidateAsync(request, null, TestContext.Current.CancellationToken);
        // Assert
        result.ShouldHaveValidationErrorFor(req => req.UserDto.Email);
    }

    [Theory]
    [InlineData("", "Password is required")]
    [InlineData("t", "Password must be at least 10 characters long")]
    [InlineData("test123!", "Password must contain at least one uppercase letter")]
    [InlineData("TEST123!", "Password must contain at least one lowercase letter")]
    [InlineData("Test!", "Password must contain at least one digit")]
    [InlineData("Test123", "Password must contain at least one special character")]
    public async Task Validator_ShouldHaveError_WhenPasswordDoesNotMetConditions(string password, string expectedErrorMessage)
    {
        // Arrange
        IQueryable<User> users = new List<User>().AsQueryable();

        Mock<IUserStore<User>> userStoreMock = new();
        Mock<UserManager<User>> userManagerMock = new(userStoreMock.Object, null, null, null, null, null, null, null, null);

        userManagerMock.Setup(um => um.Users).Returns(users);

        UserRegistration.Validator validator = new(userManagerMock.Object);

        UserRegistration.Request request = new(new() { UserName = "ValidUser", Password = password, Email = "validTest@gmail.com" });

        // Act
        TestValidationResult<UserRegistration.Request> result = await validator.TestValidateAsync(request, null, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(req => req.UserDto.Password)
              .WithErrorMessage(expectedErrorMessage);
    }
}
