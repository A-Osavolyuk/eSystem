namespace eShop.Auth.Api.Interfaces;

internal interface ITokenHandler
{
    public Task<Token> GenerateTokenAsync(AppUser user, List<string> roles, List<string> permissions);
    public Task<string> RefreshTokenAsync(AppUser user, string token);
}