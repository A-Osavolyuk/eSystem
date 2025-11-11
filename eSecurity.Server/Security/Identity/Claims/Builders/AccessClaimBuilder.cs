using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Builders;

public sealed class AccessClaimBuilder : JwtClaimBuilderBase<AccessClaimBuilder>
{
    private AccessClaimBuilder() {}
    public static AccessClaimBuilder Create() => new();

    public AccessClaimBuilder WithRole(string role) => Add(AppClaimTypes.Role, role);

    public AccessClaimBuilder WithRoles(IEnumerable<string> roles)
    {
        foreach (var role in roles) Add(AppClaimTypes.Role, role);
        return this;
    }

    public AccessClaimBuilder WithPermission(string permission) => Add(AppClaimTypes.Permission, permission);
    public AccessClaimBuilder WithPermissions(IEnumerable<string> permissions)
    {
        foreach (var p in permissions) Add(AppClaimTypes.Permission, p);
        return this;
    }

    public AccessClaimBuilder WithScope(string scopes) => Add(AppClaimTypes.Scope, scopes);
    public AccessClaimBuilder WithScope(IEnumerable<string> scopes)
        => Add(AppClaimTypes.Scope, string.Join(" ", scopes));
    
    public AccessClaimBuilder WithNotBefore(DateTimeOffset notBefore) => Add(AppClaimTypes.Nbf, notBefore);
    public AccessClaimBuilder WithAuthorizedParty(string azp) => Add(AppClaimTypes.Azp, azp);
    public AccessClaimBuilder WithAuthenticationContext(string context) => Add(AppClaimTypes.Acr, context);
    public AccessClaimBuilder WithAuthenticationMethods(IEnumerable<string> methods)
    {
        foreach (var m in methods) Add(AppClaimTypes.Amr, m);
        return this;
    }
}