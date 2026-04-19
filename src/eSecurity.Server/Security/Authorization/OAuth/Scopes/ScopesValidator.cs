namespace eSecurity.Server.Security.Authorization.OAuth.Scopes;

public static class ScopesValidator
{
    public static bool Validate(IEnumerable<string> sources, IEnumerable<string> scopes,
        out IEnumerable<string> invalidScopes)
    {
        invalidScopes = scopes.Except(sources);
        return !invalidScopes.Any();
    }
}