namespace eShop.Domain.Interfaces.Client;

public interface ITokenProvider
{
    public ValueTask<string> GetTokenAsync();
    public ValueTask SetTokenAsync(string refreshToken);
    public ValueTask RemoveAsync();
}