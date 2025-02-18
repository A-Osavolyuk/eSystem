namespace eShop.Domain.Interfaces;

public interface IIdentifiable<TKey>
{
    public TKey Id { get; init; }
}