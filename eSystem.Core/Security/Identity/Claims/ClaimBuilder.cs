using System.Security.Claims;

namespace eSystem.Core.Security.Identity.Claims;

public sealed class ClaimBuilder
{
    private ClaimBuilder() {}

    private List<Claim> Claims { get; set; } = [];

    public static ClaimBuilder Create() => new();
    
    public ClaimBuilder WithClaim(string type, string value)
    {
        Claims.Add(new Claim(type, value));
        return this;
    }
    
    public ClaimBuilder WithClaim(string type, DateTimeOffset time)
    {
        var unixSeconds = time.ToUnixTimeSeconds();
        Claims.Add(new Claim(type, unixSeconds.ToString()));
        return this;
    }

    public ClaimBuilder WithAudience(string audience)
    {
        Claims.Add(new Claim(AppClaimTypes.Aud, audience));
        return this;
    }

    public ClaimBuilder WithIssuer(string issuer)
    {
        Claims.Add(new Claim(AppClaimTypes.Iss, issuer));
        return this;
    }
    
    public ClaimBuilder WithSubject(string subject)
    {
        Claims.Add(new Claim(AppClaimTypes.Sub, subject));
        return this;
    }
    
    public ClaimBuilder WithTokenId(string tokenId)
    {
        Claims.Add(new Claim(AppClaimTypes.Jti, tokenId));
        return this;
    }
    
    public ClaimBuilder WithRole(string role)
    {
        Claims.Add(new Claim(AppClaimTypes.Role, role));
        return this;
    }
    
    public ClaimBuilder WithRoles(IEnumerable<string> roles)
    {
        foreach (var role in roles)
        {
            Claims.Add(new Claim(AppClaimTypes.Role, role));
        }
        
        return this;
    }
    
    public ClaimBuilder WithPermission(string permission)
    {
        Claims.Add(new Claim(AppClaimTypes.Permission, permission));
        return this;
    }
    
    public ClaimBuilder WithPermissions(IEnumerable<string> permissions)
    {
        foreach (var permission in permissions)
        {
            Claims.Add(new Claim(AppClaimTypes.Permission, permission));
        }
        
        return this;
    }
    
    public ClaimBuilder WithScope(string scope)
    {
        Claims.Add(new Claim(AppClaimTypes.Scope, scope));
        return this;
    }
    
    public ClaimBuilder WithScope(IEnumerable<string> scopes)
    {
        Claims.Add(new Claim(AppClaimTypes.Scope, string.Join(" ", scopes)));
        return this;
    }
    
    public ClaimBuilder WithNonce(string nonce)
    {
        Claims.Add(new Claim(AppClaimTypes.Nonce, nonce));
        return this;
    }
    
    public ClaimBuilder WithAddress(string address)
    {
        Claims.Add(new Claim(AppClaimTypes.Address, address));
        return this;
    }
    
    public ClaimBuilder WithName(string name)
    {
        Claims.Add(new Claim(AppClaimTypes.Name, name));
        return this;
    }
    
    public ClaimBuilder WithGender(string gender)
    {
        Claims.Add(new Claim(AppClaimTypes.Gender, gender));
        return this;
    }
    
    public ClaimBuilder WithNickname(string nickname)
    {
        Claims.Add(new Claim(AppClaimTypes.Nickname, nickname));
        return this;
    }
    
    public ClaimBuilder WithGivenName(string givenName)
    {
        Claims.Add(new Claim(AppClaimTypes.GivenName, givenName));
        return this;
    }
    
    public ClaimBuilder WithFamilyName(string familyName)
    {
        Claims.Add(new Claim(AppClaimTypes.FamilyName, familyName));
        return this;
    }
    
    public ClaimBuilder WithMiddleName(string middleName)
    {
        Claims.Add(new Claim(AppClaimTypes.MiddleName, middleName));
        return this;
    }
    
    public ClaimBuilder WithPicture(string picture)
    {
        Claims.Add(new Claim(AppClaimTypes.Picture, picture));
        return this;
    }
    
    public ClaimBuilder WithProfile(string profile)
    {
        Claims.Add(new Claim(AppClaimTypes.Profile, profile));
        return this;
    }
    
    public ClaimBuilder WithLocale(string locale)
    {
        Claims.Add(new Claim(AppClaimTypes.Locale, locale));
        return this;
    }
    
    public ClaimBuilder WithZoneInfo(string zoneInfo)
    {
        Claims.Add(new Claim(AppClaimTypes.ZoneInfo, zoneInfo));
        return this;
    }
    
    public ClaimBuilder WithUpdatedTime(DateTimeOffset updatedTime)
    {
        var unixSeconds = updatedTime.ToUnixTimeSeconds();
        Claims.Add(new Claim(AppClaimTypes.UpdatedAt, unixSeconds.ToString()));
        return this;
    }
    
    public ClaimBuilder WithBirthDate(DateTimeOffset updatedTime)
    {
        var unixSeconds = updatedTime.ToUnixTimeSeconds();
        Claims.Add(new Claim(AppClaimTypes.BirthDate, unixSeconds.ToString()));
        return this;
    }
    
    public ClaimBuilder WithPreferredUsername(string username)
    {
        Claims.Add(new Claim(AppClaimTypes.PreferredUsername, username));
        return this;
    }
    
    public ClaimBuilder WithEmail(string email)
    {
        Claims.Add(new Claim(AppClaimTypes.Email, email));
        return this;
    }
    
    public ClaimBuilder WithEmailVerified(bool verified)
    {
        Claims.Add(new Claim(AppClaimTypes.EmailVerified, verified.ToString()));
        return this;
    }
    
    public ClaimBuilder WithPhoneNumber(string phoneNumber)
    {
        Claims.Add(new Claim(AppClaimTypes.PhoneNumber, phoneNumber));
        return this;
    }
    
    public ClaimBuilder WithPhoneNumberVerified(bool verified)
    {
        Claims.Add(new Claim(AppClaimTypes.PhoneNumberVerified, verified.ToString()));
        return this;
    }
    
    public ClaimBuilder WithSessionId(string sessionId)
    {
        Claims.Add(new Claim(AppClaimTypes.Sid, sessionId));
        return this;
    }
    
    public ClaimBuilder WithAuthorizedParty(string authorizedParty)
    {
        Claims.Add(new Claim(AppClaimTypes.Azp, authorizedParty));
        return this;
    }
    
    public ClaimBuilder WithAuthenticationMethods(IEnumerable<string> methods)
    {
        foreach (var method in methods)
        {
            Claims.Add(new Claim(AppClaimTypes.Amr, method));
        }
        
        return this;
    }
    
    public ClaimBuilder WithAuthenticationContext(string context)
    {
        Claims.Add(new Claim(AppClaimTypes.Acr, context));
        return this;
    }
    
    public ClaimBuilder WithAccessTokenHash(string hash)
    {
        Claims.Add(new Claim(AppClaimTypes.AccessTokenHash, hash));
        return this;
    }
    
    public ClaimBuilder WithAuthorizationCodeHash(string hash)
    {
        Claims.Add(new Claim(AppClaimTypes.AuthorizationCodeHash, hash));
        return this;
    }
    
    public ClaimBuilder WithAuthenticationTime(DateTimeOffset authenticationTime)
    {
        var unixSeconds = authenticationTime.ToUnixTimeSeconds();
        Claims.Add(new Claim(AppClaimTypes.AuthenticationTime, unixSeconds.ToString()));
        return this;
    }
    
    public ClaimBuilder WithNotBefore(DateTimeOffset notBeforeTime)
    {
        var unixSeconds = notBeforeTime.ToUnixTimeSeconds();
        Claims.Add(new Claim(AppClaimTypes.Nbf, unixSeconds.ToString()));
        return this;
    }
    
    public ClaimBuilder WithIssuedTime(DateTimeOffset issuedTime)
    {
        var unixSeconds = issuedTime.ToUnixTimeSeconds();
        Claims.Add(new Claim(AppClaimTypes.Iat, unixSeconds.ToString()));
        return this;
    }
    
    public ClaimBuilder WithExpirationTime(DateTimeOffset expirationTime)
    {
        var unixSeconds = expirationTime.ToUnixTimeSeconds();
        Claims.Add(new Claim(AppClaimTypes.Exp, unixSeconds.ToString()));
        return this;
    }

    public IEnumerable<Claim> Build() => Claims;
}