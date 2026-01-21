using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public class AccessTokenValidator(
    IClientManager clientManager,
    ICertificateProvider certificateProvider,
    IOptions<TokenOptions> options) : IJwtTokenValidator
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ICertificateProvider _certificateProvider = certificateProvider;
    private readonly TokenOptions _tokenOptions = options.Value;
    private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

    public async Task<TokenValidationResult> ValidateAsync(JwtSecurityToken token, 
        CancellationToken cancellationToken = default)
    {
        var kid = Guid.Parse(token.Header.Kid);
        var certificate = await _certificateProvider.FindByIdAsync(kid, cancellationToken);
        if (certificate is null) return TokenValidationResult.Fail();

        var publicKey = certificate.Certificate.GetRSAPublicKey()!;
        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _tokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudiences = await _clientManager.GetAudiencesAsync(cancellationToken),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        try
        {
            var validationResult = await _handler.ValidateTokenAsync(token, validationParameters);
            return !validationResult.IsValid 
                ? TokenValidationResult.Fail() 
                : TokenValidationResult.Success(new ClaimsPrincipal(validationResult.ClaimsIdentity));
        }
        catch (Exception)
        {
            return TokenValidationResult.Fail();
        }
    }
}