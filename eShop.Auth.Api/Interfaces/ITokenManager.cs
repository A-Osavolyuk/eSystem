using eShop.Domain.Common.API;

namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public ValueTask<SecurityTokenEntity?> FindAsync(AppUser user);
    public ValueTask<Result> RemoveAsync(AppUser user);
    public ValueTask<Result> CreateAsync(AppUser user, string token, DateTime tokenExpiration);
    public Task<Token> GenerateAsync(AppUser user);
    public Task<string> RefreshAsync(AppUser user, string token);
}