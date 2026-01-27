using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public class GenericJwtTokenValidator(
    IClientManager clientManager,
    ICertificateProvider certificateProvider,
    IOptions<TokenOptions> options) : IJwtTokenValidator
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ICertificateProvider _certificateProvider = certificateProvider;
    private readonly TokenOptions _tokenOptions = options.Value;
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
        
        //TODO: Implement audience validation configuration
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _tokenOptions.Issuer,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

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