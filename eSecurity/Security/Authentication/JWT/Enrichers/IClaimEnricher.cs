using eSecurity.Security.Authentication.JWT.Payloads;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Enrichers;

public interface IClaimEnricher
{
    public void Enrich(ClaimBuilder builder, IdTokenPayload payload);
}