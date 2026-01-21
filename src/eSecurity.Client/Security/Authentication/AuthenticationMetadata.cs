using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Client.Security.Authentication;

public class AuthenticationMetadata
{
    public required List<AuthenticationToken> Tokens { get; set; }
}