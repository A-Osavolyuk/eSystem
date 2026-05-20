namespace eSecurity.Idp.Security.Authorization.Authorize.Par;

public static class ParHelper
{
    private const string Specification = "urn:ietf:params:oauth:request_uri";
    
    public static string GetRequestUri() => $"{Specification}:{Guid.CreateVersion7()}";
}