namespace eShop.Domain.Abstraction.Data;

public abstract class Seed<TEntity, TKey> 
    where TEntity : IEntity<TKey>
{
    protected abstract List<TEntity> Get();
}

public abstract class Seed<TEntity> 
    where TEntity : IEntity
{
    protected abstract List<TEntity> Get();
}