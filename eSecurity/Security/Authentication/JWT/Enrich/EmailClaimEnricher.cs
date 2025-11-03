using eSecurity.Security.Authentication.JWT.Id;
using eSystem.Core.Security.Authentication.ODIC.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Enrich;

public class EmailClaimEnricher : IClaimEnricher
{
    public void Enrich(ClaimBuilder builder, IdTokenPayload payload)
    {
        if (!payload.Scopes.Contains(Scopes.Email)) return;
        
        if (!string.IsNullOrEmpty(payload.Email))
            builder.WithEmail(payload.Email);

        builder.WithEmailVerified(payload.EmailVerified);
    }
}