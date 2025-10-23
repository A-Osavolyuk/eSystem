using eSystem.Domain.Abstraction.Data.Entities;

namespace eSystem.Domain.Abstraction.Data.Seeding;

public abstract class Seed<TEntity> where TEntity : Entity
{
    public abstract List<TEntity> Get();
}