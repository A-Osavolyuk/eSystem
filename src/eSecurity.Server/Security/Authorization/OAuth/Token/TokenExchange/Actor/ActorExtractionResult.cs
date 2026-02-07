using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Extraction;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Actor;

public sealed class ActorExtractionResult : ExtractionResult<ActorClaim>
{
    public static ActorExtractionResult Success(ActorClaim actor)
        => ExtractionResult<ActorClaim>.Success<ActorExtractionResult>(actor);
    
    public static ActorExtractionResult Fail()
        => ExtractionResult<ActorClaim>.Fail<ActorExtractionResult>();
}