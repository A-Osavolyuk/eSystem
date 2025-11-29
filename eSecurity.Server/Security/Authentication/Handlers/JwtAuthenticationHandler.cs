using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Common.Http.Constants;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers;

public class JwtAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationSchemeOptions> options, 
    ILoggerFactory logger, 
    UrlEncoder encoder,
    ICertificateProvider certificateProvider,
    IClientManager clientManager,
    ITokenManager tokenManager,
    IOptions<TokenOptions> tokenOptions) : AuthenticationHandler<JwtAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly ICertificateProvider _certificateProvider = certificateProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(header) || !header.StartsWith($"{AuthenticationTypes.Bearer} ")) 
            return AuthenticateResult.NoResult();
        
        var token = header.Split(" ").Last();
        if (await _tokenManager.IsOpaqueAsync(token))
        {
            var opaqueToken = await _tokenManager.FindByTokenAsync(token);
            if (opaqueToken is null || !opaqueToken.IsValid || opaqueToken.TokenType == OpaqueTokenType.RefreshToken) 
                return AuthenticateResult.Fail("Invalid token.");

            var scopes = opaqueToken.Scopes.Select(x => x.Scope.Name);
            var claims = new List<Claim>()
            {
                new (AppClaimTypes.Jti, opaqueToken.Id.ToString()),
                new (AppClaimTypes.Aud, opaqueToken.Client.Audience),
                new (AppClaimTypes.Iss, _tokenOptions.Issuer),
                new (AppClaimTypes.Sub, opaqueToken.Session.Device.UserId.ToString()),
                new (AppClaimTypes.Iat, opaqueToken.CreateDate!.Value.ToUnixTimeSeconds().ToString()),
                new (AppClaimTypes.Exp, opaqueToken.ExpiredDate.ToUnixTimeSeconds().ToString()),
                new (AppClaimTypes.Scope, string.Join(" ", scopes)),
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
        
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
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}