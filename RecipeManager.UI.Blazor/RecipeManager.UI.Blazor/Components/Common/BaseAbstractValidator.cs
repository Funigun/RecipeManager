using FluentValidation;

namespace RecipeManager.UI.Blazor.Components.Common;

public abstract class BaseAbstractValidator<T> : AbstractValidator<T>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        FluentValidation.Results.ValidationResult? result = await ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model, x => x.IncludeProperties(propertyName)));
        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}
