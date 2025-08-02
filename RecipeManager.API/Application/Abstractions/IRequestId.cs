namespace RecipeManager.Api.Application.Abstractions;

public interface IRequestId<TRequest>
           where TRequest : IRequestId<TRequest>, new()
{
    Guid Value { get; set; }

    static bool TryParse(string? input, out TRequest id)
    {
        if (Guid.TryParse(input, out Guid guid))
        {
            id = new TRequest();
            id.Value = guid;
            return true;
        }

        id = default;
        return false;
    }
}
