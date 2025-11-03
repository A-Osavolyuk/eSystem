using eSecurity.Security.Authentication.JWT.Id;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Enrich;

public interface IClaimEnricher
{
    public void Enrich(ClaimBuilder builder, IdTokenPayload payload);
}