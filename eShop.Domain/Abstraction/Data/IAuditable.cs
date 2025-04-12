namespace eShop.Domain.Abstraction.Data;

public interface IAuditable
{
    DateTime CreateDate { get; set; }
    DateTime UpdateDate { get; set; }
}
