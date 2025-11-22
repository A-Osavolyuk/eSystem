using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Common.Http.Constants;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers;

public class JwtAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationSchemeOptions> options, 
    ILoggerFactory logger, 
    UrlEncoder encoder,
    ICertificateProvider certificateProvider,
    IClientManager clientManager,
    IOptions<TokenOptions> tokenOptions) : AuthenticationHandler<JwtAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly ICertificateProvider _certificateProvider = certificateProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(header) || !header.StartsWith($"{AuthenticationTypes.Bearer} ")) 
            return AuthenticateResult.NoResult();
        
        var token = header[$"{AuthenticationTypes.Bearer} ".Length..];
        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.ReadJwtToken(token);
        if (securityToken is null) return AuthenticateResult.Fail("Invalid token.");

        var kid = Guid.Parse(securityToken.Header.Kid);
        var certificate = await _certificateProvider.FindByIdAsync(kid);
        if (certificate is null) return AuthenticateResult.Fail("Certificate not found.");
        
        var publicKey = certificate.Certificate.GetRSAPublicKey()!;
        var audiences = await _clientManager.GetAudiencesAsync();
        
        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _tokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudiences = audiences,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
        
        var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}