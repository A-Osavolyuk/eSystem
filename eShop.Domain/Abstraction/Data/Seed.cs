namespace eShop.Domain.Abstraction.Data;

public abstract class Seed<TEntity> 
    where TEntity : IEntity
{
    public abstract List<TEntity> Get();
}