using eShop.Domain.Common.Security;

namespace eShop.Infrastructure.Security;

public class JwtAuthenticationState
{
    public Guid UserId
    {
        get
        {
            var value = Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Id)?.Value!;
            return Guid.Parse(value);
        }
    }

    public required List<Claim> Claims { get; init; }
    public bool IsAuthenticated => Claims.Count > 0;

    public bool HasRole(string role)
    {
        if (Claims.Count == 0) return false;
        if (string.IsNullOrEmpty(role)) return true;

        var roleClaims = Claims.Where(c => c.Type == AppClaimTypes.Role).ToList();
        return roleClaims.Count > 0 && roleClaims.Any(x => x.Value == role);
    }

    public bool HasRole(List<string> roles)
    {
        if (Claims.Count == 0) return false;
        if (roles.Count == 0) return true;

        var roleClaims = Claims.Where(c => c.Type == AppClaimTypes.Role).ToList();
        return roleClaims.Count > 0 && roles.Intersect(roleClaims.Select(x => x.Value)).Any();
    }

    public bool HasPermission(string permission)
    {
        if (Claims.Count == 0) return false;
        if (string.IsNullOrEmpty(permission)) return true;

        var permissionClaims = Claims.Where(c => c.Type == AppClaimTypes.Permission).ToList();
        return permissionClaims.Count > 0 && permissionClaims.Any(x => x.Value == permission);
    }

    public bool HasPermission(List<string> permissions)
    {
        if (Claims.Count == 0) return false;
        if (permissions.Count == 0) return true;

        var permissionClaims = Claims.Where(c => c.Type == AppClaimTypes.Permission).ToList();
        return permissionClaims.Count > 0 && permissions.Intersect(permissionClaims.Select(x => x.Value)).Any();
    }
}