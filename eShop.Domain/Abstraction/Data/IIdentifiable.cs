namespace eShop.Domain.Abstraction.Data;

public interface IIdentifiable<TKey>
{
    public TKey Id { get; init; }
}