using System.Reflection;

namespace RecipeManager.Shared.Contracts;
public static class AssemblyReader
{
    public static Assembly GetAssembly() => Assembly.GetExecutingAssembly();
}
