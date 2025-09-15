namespace eShop.Blazor.Domain.Interfaces;

public interface ITokenProvider
{
    public ValueTask<string?> GetAsync();
}