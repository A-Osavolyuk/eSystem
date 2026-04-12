namespace eSystem.Core.Data.Entities;

public abstract class Entity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}