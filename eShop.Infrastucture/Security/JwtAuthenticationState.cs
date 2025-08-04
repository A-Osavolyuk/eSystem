using eShop.Domain.Common.Security;

namespace eShop.Infrastructure.Security;

public class JwtAuthenticationState
{
    public required string AuthenticationType { get; init; }
    public required List<Claim> Claims { get; init; }
    public bool IsAuthenticated => Claims.Count > 0;

    public bool HasRole(List<string> roles)
    {
        if (Claims.Count == 0) return false;
        if (roles.Count == 0) return true;

        var roleClaims = Claims.Where(c => c.Type == AppClaimTypes.Role).ToList();
        return roleClaims.Count > 0 && roles.Intersect(roleClaims.Select(x => x.Value)).Any();
    }

    public bool HasPermission(List<string> permissions)
    {
        if (Claims.Count == 0) return false;
        if (permissions.Count == 0) return true;

        var permissionClaims = Claims.Where(c => c.Type == AppClaimTypes.Permission).ToList();
        return permissionClaims.Count > 0 && permissions.Intersect(permissionClaims.Select(x => x.Value)).Any();
    }
}