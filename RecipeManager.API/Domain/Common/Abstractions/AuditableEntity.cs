namespace RecipeManager.API.Domain.Common.Abstractions;

public abstract class AuditableEntity
{
    public string CreatedBy { get; set; } = default!;

    public DateTime CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
