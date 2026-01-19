using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.Oidc.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eCinema.Server.Security.Authentication.Oidc.Token;

public class TokenValidator(
    IOpenIdDiscoveryProvider openIdDiscoveryProvider,
    IOptions<ClientOptions> options) : ITokenValidator
{
    private readonly IOpenIdDiscoveryProvider _openIdDiscoveryProvider = openIdDiscoveryProvider;
    private readonly ClientOptions _clientOptions = options.Value;

    public async Task<Result> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        var jsonWebKeySet = await _openIdDiscoveryProvider.GetWebKeySetAsync(cancellationToken);
        if (jsonWebKeySet is null)
        {
            return Results.InternalServerError(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
        
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var securityToken = handler.ReadJwtToken(token);
        if (securityToken is null) return Results.BadRequest("Invalid token.");
        
        var publicKey = jsonWebKeySet.Keys.FirstOrDefault(x => x.KeyId == securityToken.Header.Kid);
        if (publicKey is null) return Results.BadRequest("Invalid key.");
        
        var openIdConfiguration = await _openIdDiscoveryProvider.GetOpenIdConfigurationsAsync(cancellationToken);
        if (openIdConfiguration is null)
        {
            return Results.InternalServerError(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
        
        var parameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = openIdConfiguration.Issuer,
            ValidateAudience = true,
            ValidAudience = _clientOptions.ClientAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(CreateRsaFromJwk(publicKey)),
        };
        
        var principal = handler.ValidateToken(token, parameters, out _);
        return principal is null ? Results.BadRequest("Invalid token.") : Results.Ok();
    }
    
    private RSA CreateRsaFromJwk(JsonWebKey jwk)
    {
        var rsa = RSA.Create();

        var parameters = new RSAParameters
        {
            Modulus = Base64UrlEncoder.DecodeBytes(jwk.N),
            Exponent = Base64UrlEncoder.DecodeBytes(jwk.E)
        };

        rsa.ImportParameters(parameters);
        return rsa;
    }
}