using eSecurity.Security.Authentication.JWT.Payloads;
using eSystem.Core.Security.Authentication.JWT.Claims;
using eSystem.Core.Security.Authentication.ODIC.Constants;

namespace eSecurity.Security.Authentication.JWT.Enrichers;

public class AddressClaimEnricher : IClaimEnricher
{
    public void Enrich(ClaimBuilder builder, IdTokenPayload payload)
    {
        if (!payload.Scopes.Contains(Scopes.Address)) return;
        
        //TODO: Implement address scope handling
    }
}