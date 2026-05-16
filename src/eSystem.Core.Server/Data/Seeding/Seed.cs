using eSystem.Core.Server.Data.Entities;

namespace eSystem.Core.Server.Data.Seeding;

public abstract class Seed<TEntity> where TEntity : Entity
{
    public abstract List<TEntity> Get();
}