namespace eShop.Domain.Interfaces.Client;

public interface ITokenProvider
{
    public ValueTask<string?> GetAsync();
    public ValueTask SetAsync(string token);
    public ValueTask RemoveAsync();
}