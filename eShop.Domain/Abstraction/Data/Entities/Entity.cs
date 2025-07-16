namespace eShop.Domain.Abstraction.Data.Entities;

public abstract class Entity
{
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}