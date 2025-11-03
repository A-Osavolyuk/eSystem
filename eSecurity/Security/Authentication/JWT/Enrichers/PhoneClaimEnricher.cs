using eSecurity.Security.Authentication.JWT.Payloads;
using eSystem.Core.Security.Authentication.JWT.Claims;
using eSystem.Core.Security.Authentication.ODIC.Constants;

namespace eSecurity.Security.Authentication.JWT.Enrichers;

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