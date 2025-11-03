using eSecurity.Security.Authentication.JWT.Id;
using eSystem.Core.Security.Authentication.ODIC.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Enrich;

public class PhoneClaimEnricher : IClaimEnricher
{
    public void Enrich(ClaimBuilder builder, IdTokenPayload payload)
    {
        if (!payload.Scopes.Contains(Scopes.Phone)) return;
        
        if (!string.IsNullOrEmpty(payload.PhoneNumber))
            builder.WithPhoneNumber(payload.PhoneNumber);

        builder.WithPhoneNumberVerified(payload.PhoneNumberVerified);
    }
}