namespace eShop.Domain.Interfaces;

public interface IAuditable
{
    DateTime CreateDate { get; set; }
    DateTime UpdateDate { get; set; }
}
