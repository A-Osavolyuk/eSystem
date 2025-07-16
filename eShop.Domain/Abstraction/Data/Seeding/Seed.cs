namespace eShop.Domain.Abstraction.Data.Seeding;

public abstract class Seed<TEntity>
{
    public abstract List<TEntity> Get();
}