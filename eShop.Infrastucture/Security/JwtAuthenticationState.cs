using eShop.Domain.Common.Security;

namespace eShop.Infrastructure.Security;

public class JwtAuthenticationState
{
    public required List<Claim> Claims { get; init; }
    public bool IsAuthenticated => Claims.Count > 0;

    public Guid? GetId()
    {
        var id = Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Id)?.Value;
        return string.IsNullOrEmpty(id) ? null : Guid.Parse(id);
    }

    public bool HasRole(string role)
    {
        if (string.IsNullOrEmpty(role)) return true;

        var roleClaims = Claims.Where(c => c.Type == AppClaimTypes.Role).ToList();
        return roleClaims.Count > 0 && roleClaims.Any(x => x.Value == role);
    }

    public bool HasRole(List<string> roles)
    {
        if (roles.Count == 0) return true;

        var roleClaims = Claims.Where(c => c.Type == AppClaimTypes.Role).ToList();
        return roleClaims.Count > 0 && roles.Intersect(roleClaims.Select(x => x.Value)).Any();
    }

    public bool HasPermission(string permission)
    {
        if (string.IsNullOrEmpty(permission)) return true;

        var permissionClaims = Claims.Where(c => c.Type == AppClaimTypes.Permission).ToList();
        return permissionClaims.Count > 0 && permissionClaims.Any(x => x.Value == permission);
    }

    public bool HasPermission(List<string> permissions)
    {
        if (permissions.Count == 0) return true;

        var permissionClaims = Claims.Where(c => c.Type == AppClaimTypes.Permission).ToList();
        return permissionClaims.Count > 0 && permissions.Intersect(permissionClaims.Select(x => x.Value)).Any();
    }
}