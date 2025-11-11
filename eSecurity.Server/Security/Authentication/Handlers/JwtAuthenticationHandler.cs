using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers;

public class JwtAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationSchemeOptions> options, 
    ILoggerFactory logger, 
    UrlEncoder encoder) : AuthenticationHandler<JwtAuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //TODO: Implement JWT authentication handler
        
        throw new NotImplementedException();
    }
}