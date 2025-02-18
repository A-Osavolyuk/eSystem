namespace eShop.Domain.Interfaces;

public interface IAuditable
{
    DateTime CreateDate { get; init; }
    DateTime UpdateDate { get; init; }
}
