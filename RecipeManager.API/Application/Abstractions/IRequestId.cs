namespace RecipeManager.Api.Application.Abstractions;

public interface IRequestId<TRequest>
           where TRequest : IRequestId<TRequest>, new()
{
    Guid Id { get; set; }

    bool TryParse(string? input, out TRequest id)
    {
        if (Guid.TryParse(input, out Guid guid))
        {
            id = new()
            {
                Id = guid
            };
            return true;
        }

        id = default!;
        return false;
    }
}
