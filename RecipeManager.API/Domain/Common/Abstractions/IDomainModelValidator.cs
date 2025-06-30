namespace RecipeManager.API.Domain.Common.Abstractions;

public interface IDomainModelValidator<in TEntity>
           where TEntity : class, IEntity
{
    void Validate(TEntity entity);
}
