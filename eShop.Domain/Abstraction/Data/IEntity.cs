namespace eShop.Domain.Abstraction.Data;

public interface IEntity
{
    DateTimeOffset? CreateDate { get; set; }
    DateTimeOffset? UpdateDate { get; set; }
}