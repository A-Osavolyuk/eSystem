namespace eShop.Blazor.Domain.Interfaces;

public interface ITokenProvider
{
    public ValueTask<string?> GetAsync();
    public ValueTask SetAsync(string token);
    public ValueTask RemoveAsync();
}