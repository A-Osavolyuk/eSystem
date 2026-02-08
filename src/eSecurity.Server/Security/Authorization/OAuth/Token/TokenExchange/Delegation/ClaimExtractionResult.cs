using System.Security.Claims;
using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Extraction;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public sealed class ClaimExtractionResult : ExtractionResult<IEnumerable<Claim>>
{
    public static ClaimExtractionResult Success(IEnumerable<Claim> claims)
        => ExtractionResult<IEnumerable<Claim>>.Success<ClaimExtractionResult>(claims);

    public static ClaimExtractionResult Fail()
        => ExtractionResult<IEnumerable<Claim>>.Fail<ClaimExtractionResult>();
}