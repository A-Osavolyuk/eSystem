using eSecurity.Security.Authentication.JWT.Id;
using eSystem.Core.Security.Authentication.ODIC.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Enrich;

public class AddressClaimEnricher : IClaimEnricher
{
    public void Enrich(ClaimBuilder builder, IdTokenPayload payload)
    {
        if (!payload.Scopes.Contains(Scopes.Address)) return;
        
        //TODO: Implement address scope handling
    }
}