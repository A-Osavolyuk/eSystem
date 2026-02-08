using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Extraction;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public interface ITokenClaimsExtractor : ITokenExtractor<ClaimExtractionResult>;