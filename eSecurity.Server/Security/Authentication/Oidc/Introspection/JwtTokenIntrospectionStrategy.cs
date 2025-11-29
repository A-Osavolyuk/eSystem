using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.Oidc.Introspection;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authentication.Oidc.Introspection;

public sealed class JwtTokenIntrospectionStrategy(
    ICertificateProvider certificateProvider,
    IClientManager clientManager,
    ISessionManager sessionManager,
    IUserManager userManager,
    IOptions<TokenOptions> tokenOptions) : IIntrospectionStrategy
{
    private readonly ICertificateProvider _certificateProvider = certificateProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;

    public async ValueTask<Result> ExecuteAsync(IntrospectionContext 
        context, CancellationToken cancellationToken = default)
    {
        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.ReadJwtToken(context.Token);
        if (securityToken is null) return Results.Ok(IntrospectionResponse.Fail());

        var kid = Guid.Parse(securityToken.Header.Kid);
        var certificate = await _certificateProvider.FindByIdAsync(kid, cancellationToken);
        if (certificate is null)
            return Results.InternalServerError(new Error()
            {
                Code = Errors.OAuth.ServerError,
                Description = "Server error."
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
            var principal = handler.ValidateToken(context.Token, validationParameters, out var validatedToken);
            if (validatedToken is null || principal is null) return Results.Ok(IntrospectionResponse.Fail());
            
            var sidClaim = principal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Sid);
            if (sidClaim is null) return Results.Ok(IntrospectionResponse.Fail());

            var session = await _sessionManager.FindByIdAsync(Guid.Parse(sidClaim.Value), cancellationToken);
            if (session is null) return Results.Ok(IntrospectionResponse.Fail());

            var audienceClaim = principal.Claims.First(x => x.Type == AppClaimTypes.Aud);
            var client = await _clientManager.FindByIdAsync(audienceClaim.Value, cancellationToken);
            if (client is null) return Results.Ok(IntrospectionResponse.Fail());
        
            var subjectClaim = principal.Claims.First(x => x.Type == AppClaimTypes.Sub);
            var user = await _userManager.FindByIdAsync(Guid.Parse(subjectClaim.Value), cancellationToken);
            if (user is null) return Results.Ok(IntrospectionResponse.Fail());
        
            var expirationClaim = principal.Claims.First(x => x.Type == AppClaimTypes.Exp);
            var issuedAtClaim = principal.Claims.First(x => x.Type == AppClaimTypes.Iat);
            var notBeforeClaim = principal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Nbf);

            var response = new IntrospectionResponse()
            {
                Active = true,
                Issuer = _tokenOptions.Issuer,
                Username = user.Username,
                ClientId = client.Id,
                Scope = string.Join(" ", client.AllowedScopes.Select(x => x.Scope)),
                TokenType = IntrospectionTokenTypes.AccessToken,
                Audience = audienceClaim.Value,
                NotBefore = notBeforeClaim is null ? null : long.Parse(notBeforeClaim.Value),
                Expiration = long.Parse(expirationClaim.Value),
                IssuedAt = long.Parse(issuedAtClaim.Value)
            };

            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.Ok(IntrospectionResponse.Fail());
        }
    }
}