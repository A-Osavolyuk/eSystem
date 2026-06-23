using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Idp.Security.Cryptography.Signing.Certificates;
using eSecurity.Idp.Security.Cryptography.Tokens;
using TokenValidationResult = eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation.TokenValidationResult;

namespace eSecurity.Idp.Security.Authorization.Token.Validation;

public class AccessTokenValidator(
    ICertificateProvider certificateProvider,
    IOptions<TokenConfigurations> options) : IJwtTokenValidator
{
    private readonly ICertificateProvider _certificateProvider = certificateProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

    public async Task<TokenValidationResult> ValidateAsync(string token, 
        CancellationToken cancellationToken = default)
    {
        var securityToken = _handler.ReadJwtToken(token);
        if (securityToken is null)
            return TokenValidationResult.Fail();
        
        var kid = Guid.Parse(securityToken.Header.Kid);
        var certificate = await _certificateProvider.FindByIdAsync(kid, cancellationToken);
        if (certificate is null) return TokenValidationResult.Fail();

        var publicKey = certificate.Certificate.GetRSAPublicKey()!;
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _tokenConfigurations.Issuer,
            ValidateAudience = true,
            ValidAudience = "api://esecurity",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            RequireSignedTokens = true,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256]
        };
        
        _handler.MapInboundClaims = false;

        try
        {
            var claimPrincipal = _handler.ValidateToken(token, validationParameters, out _);
            return claimPrincipal is null 
                ? TokenValidationResult.Fail() 
                : TokenValidationResult.Success(new ClaimsPrincipal(claimPrincipal));
        }
        catch (Exception)
        {
            return TokenValidationResult.Fail();
        }
    }
}