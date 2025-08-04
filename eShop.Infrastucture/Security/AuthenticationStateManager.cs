namespace eShop.Infrastructure.Security;

public class AuthenticationStateManager(
    TokenHandler tokenHandler,
    ITokenProvider tokenProvider)
{
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly TokenHandler tokenHandler = tokenHandler;
    
    public async Task<JwtAuthenticationState?> GetStateAsync()
    {
        var token = await tokenProvider.GetTokenAsync();

        if (string.IsNullOrEmpty(token)) return null;

        var rawToken = tokenHandler.ReadToken(token);
        var claims = rawToken!.Claims.ToList();
        var state = new JwtAuthenticationState()
        {
            Claims = claims,
            AuthenticationType = JwtBearerDefaults.AuthenticationScheme
        };

        return state;
    }
}