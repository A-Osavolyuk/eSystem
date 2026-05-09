namespace eSecurity.Server.Security.Authorization.Par;

public static class ParHelper
{
    private const string Specification = "urn:ietf:params:oauth:request_uri";
    
    public static string GetRequestUri(Guid parId) => $"{Specification}:{parId}";
}