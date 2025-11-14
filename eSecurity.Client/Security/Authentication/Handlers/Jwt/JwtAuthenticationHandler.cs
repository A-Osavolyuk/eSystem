using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using eSecurity.Client.Security.Authentication.Oidc;
using eSecurity.Client.Security.Authentication.Oidc.Clients;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSecurity.Core.Security.Authentication.Oidc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eSecurity.Client.Security.Authentication.Handlers.Jwt;

public class JwtAuthenticationOptions : AuthenticationSchemeOptions{}

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    TokenProvider tokenProvider,
    IConnectService connectService,
    IOptions<ClientOptions> clientOptions) : AuthenticationHandler<JwtAuthenticationOptions>(options, logger, encoder)
{
    private readonly TokenProvider _tokenProvider = tokenProvider;
    private readonly IConnectService _connectService = connectService;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var keysResult = await _connectService.GetPublicKeysAsync();
        if (!keysResult.Success) return AuthenticateResult.Fail(keysResult.Message);

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.ReadJwtToken(_tokenProvider.IdToken);
        if (securityToken is null) return AuthenticateResult.Fail("Invalid token.");
        
        var keys = new JsonWebKeySet(keysResult.Result!.ToString());
        var publicKey = keys.Keys.FirstOrDefault(x => x.KeyId == securityToken.Header.Kid);
        if (publicKey is null) return AuthenticateResult.Fail("Invalid key.");

        var openIdResult = await _connectService.GetOpenidConfigurationAsync();
        if (openIdResult.Success) return AuthenticateResult.Fail(openIdResult.Message);

        var openIdOptions = openIdResult.Get<OpenIdOptions>();
        var signingKey = new RsaSecurityKey(CreateRsaFromJwk(publicKey));
        var parameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = openIdOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _clientOptions.ClientId,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
        };
        
        var principal = handler.ValidateToken(_tokenProvider.IdToken, parameters, out _);
        if (principal is null) return AuthenticateResult.Fail("Invalid token.");
        
        var claimIdentity = new ClaimsIdentity(principal.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimPrincipal = new ClaimsPrincipal(claimIdentity);
        var ticket = new AuthenticationTicket(claimPrincipal, CookieAuthenticationDefaults.AuthenticationScheme);
        return AuthenticateResult.Success(ticket);
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