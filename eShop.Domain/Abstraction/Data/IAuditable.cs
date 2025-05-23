namespace eShop.Domain.Abstraction.Data;

public interface IAuditable
{
    DateTimeOffset? CreateDate { get; set; }
    DateTimeOffset? UpdateDate { get; set; }
}
