using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using eSecurity.Core.Common.DTOs;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.Oidc.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eSecurity.Client.Security.Authentication.Oidc.Token;

public class TokenValidator(
    IConnectService connectService,
    IOptions<ClientOptions> clientOptions) : ITokenValidator
{
    private readonly IConnectService _connectService = connectService;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    public async ValueTask<Result> ValidateAsync(string token)
    {
        var keysResult = await _connectService.GetPublicKeysAsync();
        if (!keysResult.Succeeded) return Results.InternalServerError(keysResult.GetError().Description);

        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var securityToken = handler.ReadJwtToken(token);
        if (securityToken is null) return Results.BadRequest("Invalid token.");
        
        var keys = keysResult.Get();
        var publicKey = keys.Keys.FirstOrDefault(x => x.KeyId == securityToken.Header.Kid);
        if (publicKey is null) return Results.BadRequest("Invalid key.");

        var openIdResult = await _connectService.GetOpenidConfigurationAsync();
        if (!openIdResult.Succeeded) return Results.InternalServerError(keysResult.GetError().Description);

        var openIdOptions = openIdResult.Get();
        var signingKey = new RsaSecurityKey(CreateRsaFromJwk(publicKey));
        var parameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = openIdOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _clientOptions.ClientAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
        };
        
        var principal = handler.ValidateToken(token, parameters, out _);
        if (principal is null) return Results.BadRequest("Invalid token.");

        var claims = principal.Claims.Select(x => new ClaimValue()
        {
            Type = x.Type, 
            Value = x.Value
        }).ToList();
        return Results.Ok(claims);
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