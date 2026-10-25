namespace RecipeManager.Api.Features.Units;

public sealed record CollectionActionDto(bool CanAdd);

public sealed record ItemActionDto(bool CanView, bool CanEdit, bool CanDelete);
