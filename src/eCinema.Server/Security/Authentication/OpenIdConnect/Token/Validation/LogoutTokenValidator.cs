using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using eCinema.Server.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Security.Cryptography.Encryption;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eCinema.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public sealed class LogoutTokenValidator(
    IOpenIdDiscoveryProvider discoveryProvider,
    IOptions<OAuthOptions> options) : ITokenValidator
{
    private readonly IOpenIdDiscoveryProvider _discoveryProvider = discoveryProvider;
    private readonly OAuthOptions _oauthOptions = options.Value;

    public async Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return TokenValidationResult.Fail();
        
        var securityToken = handler.ReadJwtToken(token);
        if (securityToken is null) return TokenValidationResult.Fail();
        
        var eventsClaim = securityToken.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Events);
        if (eventsClaim is null) return TokenValidationResult.Fail();
        
        var events = JsonSerializer.Deserialize<Dictionary<string, object>>(eventsClaim.Value);
        if (events is null || !events.ContainsKey(LogoutEvents.BackChannelLogout)) return TokenValidationResult.Fail();
        
        var openIdDiscovery = await _discoveryProvider.GetOpenIdDiscoveryAsync(cancellationToken);
        if (openIdDiscovery is null) return TokenValidationResult.Fail();

        var jsonWebKeySet = await _discoveryProvider.GetJsonWebKeySetAsync(cancellationToken);
        if (jsonWebKeySet is null) return TokenValidationResult.Fail();

        var key = jsonWebKeySet.Keys.FirstOrDefault(x => x.Kid == securityToken.Header.Kid);
        if (key is null) return TokenValidationResult.Fail();

        var publicKey = RsaConverter.FromJsonWebkey(key);
        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = openIdDiscovery.Issuer,
            ValidateAudience = true,
            ValidAudience = _oauthOptions.ClientId,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey),
            RequireSignedTokens = true,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256]
        };
        
        var principal = handler.ValidateToken(token, validationParameters, out _);
        return principal is null ? TokenValidationResult.Fail() : TokenValidationResult.Success(principal);
    }
}