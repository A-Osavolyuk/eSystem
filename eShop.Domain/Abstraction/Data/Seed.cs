namespace eShop.Domain.Abstraction.Data;

public abstract class Seed<TEntity, TKey> 
    where TEntity : IEntity<TKey>
{
    public abstract List<TEntity> Get();
}

public abstract class Seed<TEntity> 
    where TEntity : IEntity
{
    public abstract List<TEntity> Get();
}