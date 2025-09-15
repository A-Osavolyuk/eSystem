namespace eShop.Blazor.Domain.Interfaces;

public interface ITokenProvider
{
    public string? Get();
    public void Set(string accessToken);
    public void Clear();
}