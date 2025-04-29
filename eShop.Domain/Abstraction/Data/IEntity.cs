namespace eShop.Domain.Abstraction.Data;

public interface IEntity<TKey> : IIdentifiable<TKey>, IAuditable;
public interface IEntity :  IAuditable;