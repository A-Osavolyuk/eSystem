using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Idp.Common.Responses;

public class OAuthLoginResponse
{
    public AuthenticationProperties AuthenticationProperties { get; set; } = null!;
    public string Provider { get; set; } = string.Empty;
}