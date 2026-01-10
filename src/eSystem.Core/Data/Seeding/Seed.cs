using eSystem.Core.Data.Entities;

namespace eSystem.Core.Data.Seeding;

public abstract class Seed<TEntity> where TEntity : Entity
{
    public abstract List<TEntity> Get();
}