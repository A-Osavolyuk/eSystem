namespace eShop.Domain.Interfaces.Client;

public interface IUserStorage
{
    public ValueTask SaveAsync(Guid userId);
    public ValueTask<Guid> GetAsync();
    public ValueTask ClearAsync();
}