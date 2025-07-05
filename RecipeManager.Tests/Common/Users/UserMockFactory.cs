using Moq;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Integration.Tests.Common.Users;

internal static class UserMockFactory
{
    public static ICurrentUser CreateMockedAdmin()
    {
        return CreateMock(UserRoles.Admin);
    }

    public static ICurrentUser CreateMockedUser()
    {
        return CreateMock(UserRoles.User);
    }

    public static ICurrentUser CreateMockedGuest()
    {
        return CreateMock(UserRoles.Guest);
    }

    private static FakeCurrentUser CreateMock(string role)
    {
        Mock<FakeCurrentUser> currentUserMock = new() { CallBase = true };
        currentUserMock.Setup(m => m.Id).Returns(1);
        currentUserMock.Setup(m => m.Roles).Returns([role]);

        return currentUserMock.Object;
    }
}
