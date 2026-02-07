using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using eSecurity.Server.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Actor;

public sealed class JwtTokenActorExtractor(
    IJwtTokenValidationProvider validationProvider,
    IOptions<TokenOptions> options) : ITokenActorExtractor
{
    private readonly IJwtTokenValidationProvider _validationProvider = validationProvider;
    private readonly TokenOptions _options = options.Value;
    private readonly JwtSecurityTokenHandler _handler = new();

    public async ValueTask<ActorExtractionResult> ExtractAsync(string actorToken, CancellationToken cancellationToken)
    {
        if (!_handler.CanReadToken(actorToken))
            return ActorExtractionResult.Fail();

        var securityToken = _handler.ReadJwtToken(actorToken);
        if (securityToken is null ||
            !securityToken.Header.Typ.Equals(JwtTokenTypes.AccessToken, StringComparison.OrdinalIgnoreCase))
        {
            return ActorExtractionResult.Fail();
        }

        var validator = _validationProvider.CreateValidator(securityToken.Header.Typ);
        var validationResult = await validator.ValidateAsync(actorToken, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null)
            return ActorExtractionResult.Fail();

        var tokenClaims = validationResult.ClaimsPrincipal.Claims.ToList();
        var clientIdClaim = tokenClaims.FirstOrDefault(x => x.Type == AppClaimTypes.ClientId);
        if (clientIdClaim is null) return ActorExtractionResult.Fail();
        
        var actClaim = tokenClaims.FirstOrDefault(x => x.Type == AppClaimTypes.Act);
        var actor = new ActorClaim()
        {
            Subject = clientIdClaim.Value,
            ClientId = clientIdClaim.Value,
            Issuer = _options.Issuer,
            AuthenticationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Actor = actClaim is not null ? JsonSerializer.Deserialize<ActorClaim>(actClaim.Value) : null
        };
        
        return ActorExtractionResult.Success(actor);
    }
}