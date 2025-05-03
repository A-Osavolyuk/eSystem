namespace eShop.Domain.Abstraction.Data;

public abstract class Seed<TEntity>
{
    protected abstract List<TEntity> Get();
}