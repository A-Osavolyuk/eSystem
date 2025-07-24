using eShop.Domain.Abstraction.Data.Entities;

namespace eShop.Domain.Abstraction.Data.Seeding;

public abstract class Seed<TEntity> where TEntity : Entity
{
    public abstract List<TEntity> Get();
}