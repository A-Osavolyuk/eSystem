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
    
    public ClaimBuilder WithName(string name)
    {
        Claims.Add(new Claim(AppClaimTypes.Name, name));
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
    
    public ClaimBuilder WithPhoneNumber(string phoneNumber)
    {
        Claims.Add(new Claim(AppClaimTypes.PhoneNumber, phoneNumber));
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