using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authentication.Oidc.Token;

public class TokenValidator(
    ICertificateProvider certificateProvider,
    IClientManager clientManager,
    ITokenManager tokenManager,
    IHasherFactory hasherFactory,
    IOptions<TokenOptions> tokenOptions) : ITokenValidator
{
    private readonly ICertificateProvider _certificateProvider = certificateProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasherFactory _hasherFactory = hasherFactory;
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;

    public async ValueTask<Result> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        if (await _tokenManager.IsOpaqueAsync(token, cancellationToken))
        {
            var hasher = _hasherFactory.CreateHasher(HashAlgorithm.Sha512);
            var incomingHash = hasher.Hash(token);
            var opaqueToken = await _tokenManager.FindByTokenAsync(incomingHash, cancellationToken);
            if (opaqueToken is null || !opaqueToken.IsValid || opaqueToken.TokenType == OpaqueTokenType.RefreshToken)
                return Results.Unauthorized(new Error()
                {
                    Code = Errors.OAuth.InvalidToken,
                    Description = "Invalid token"
                });

            var scopes = opaqueToken.Scopes.Select(x => x.Scope.Name);
            var claims = new List<Claim>()
            {
                new(AppClaimTypes.Jti, opaqueToken.Id.ToString()),
                new(AppClaimTypes.Sid, opaqueToken.SessionId.ToString()),
                new(AppClaimTypes.Aud, opaqueToken.Client.Audience),
                new(AppClaimTypes.Iss, _tokenOptions.Issuer),
                new(AppClaimTypes.Sub, opaqueToken.Session.Device.UserId.ToString()),
                new(AppClaimTypes.Iat, opaqueToken.CreateDate!.Value.ToUnixTimeSeconds().ToString()),
                new(AppClaimTypes.Exp, opaqueToken.ExpiredDate.ToUnixTimeSeconds().ToString()),
                new(AppClaimTypes.Scope, string.Join(" ", scopes)),
            };

            var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return Results.Ok(claimsPrincipal);
        }

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.ReadJwtToken(token);
        if (securityToken is null)
            return Results.Unauthorized(new Error()
            {
                Code = Errors.OAuth.InvalidToken,
                Description = "Invalid token"
            });

        var kid = Guid.Parse(securityToken.Header.Kid);
        var certificate = await _certificateProvider.FindByIdAsync(kid, cancellationToken);
        if (certificate is null)
            return Results.Unauthorized(new Error()
            {
                Code = Errors.OAuth.InvalidToken,
                Description = "Invalid token"
            });

        var publicKey = certificate.Certificate.GetRSAPublicKey()!;
        var audiences = await _clientManager.GetAudiencesAsync(cancellationToken);

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

        try
        {
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
            return Results.Ok(principal);
        }
        catch (Exception)
        {
            return Results.InternalServerError(new Error()
            {
                Code = Errors.OAuth.InvalidToken,
                Description = "Invalid token"
            });
        }
    }
}