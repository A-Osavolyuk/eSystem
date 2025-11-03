using eSecurity.Security.Authentication.JWT.Payloads;
using eSystem.Core.Security.Authentication.JWT.Claims;
using eSystem.Core.Security.Authentication.ODIC.Constants;

namespace eSecurity.Security.Authentication.JWT.Enrichers;

public class ProfileClaimEnricher : IClaimEnricher
{
    public void Enrich(ClaimBuilder builder, IdTokenPayload payload)
    {
        if (!payload.Scopes.Contains(Scopes.Profile)) return;

        if (!string.IsNullOrEmpty(payload.Name))
            builder.WithName(payload.Name);
        
        if (!string.IsNullOrEmpty(payload.Nickname))
            builder.WithNickname(payload.Nickname);
        
        if (!string.IsNullOrEmpty(payload.PreferredUsername))
            builder.WithPreferredUsername(payload.PreferredUsername);
        
        if (!string.IsNullOrEmpty(payload.GivenName))
            builder.WithGivenName(payload.GivenName);
        
        if (!string.IsNullOrEmpty(payload.FamilyName))
            builder.WithFamilyName(payload.FamilyName);
        
        if (!string.IsNullOrEmpty(payload.MiddleName))
            builder.WithMiddleName(payload.MiddleName);
        
        if (!string.IsNullOrEmpty(payload.Profile))
            builder.WithProfile(payload.Profile);
        
        if (!string.IsNullOrEmpty(payload.Picture))
            builder.WithPicture(payload.Picture);
        
        if (!string.IsNullOrEmpty(payload.Locale))
            builder.WithLocale(payload.Locale);
        
        if (!string.IsNullOrEmpty(payload.ZoneInfo))
            builder.WithZoneInfo(payload.ZoneInfo);
        
        if (!string.IsNullOrEmpty(payload.Gender))
            builder.WithGender(payload.Gender);

        if (payload.Birthdate.HasValue)
            builder.WithBirthDate(payload.Birthdate.Value);
        
        if (payload.UpdatedAt.HasValue)
            builder.WithUpdatedTime(payload.UpdatedAt.Value);
    }
}