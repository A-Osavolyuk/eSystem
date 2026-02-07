using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Extraction;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Claims;

public interface ITokenClaimsExtractor : ITokenExtractor<ClaimExtractionResult>;