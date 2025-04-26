namespace eShop.Auth.Api.Interfaces;

internal interface ITokenHandler
{
    public Task<Token> GenerateTokenAsync(UserEntity userEntity, List<string> roles, List<string> permissions);
    public Task<string> RefreshTokenAsync(UserEntity userEntity, string token);
}