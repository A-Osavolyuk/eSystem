using System.Security.Claims;
using System.Text.Encodings.Web;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSystem.Core.Common.Http.Constants;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers;

public class JwtAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationSchemeOptions> options, 
    ILoggerFactory logger, 
    UrlEncoder encoder,
    ITokenValidator tokenValidator) : AuthenticationHandler<JwtAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly ITokenValidator _tokenValidator = tokenValidator;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(header) || !header.StartsWith($"{AuthenticationTypes.Bearer} ")) 
            return AuthenticateResult.NoResult();
        
        var token = header.Split(" ").Last();
        var result = await _tokenValidator.ValidateAsync(token);
        
        if (!result.Succeeded)
        {
            var error = result.GetError();
            Context.Items["error"] = error.Code;
            
            return AuthenticateResult.Fail(error.Description);
        }
        
        var claimPrincipal = result.Get<ClaimsPrincipal>();
        var ticket = new AuthenticationTicket(claimPrincipal, JwtBearerDefaults.AuthenticationScheme);
            
        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var error = Context.Items["error"]?.ToString();

        if (string.IsNullOrEmpty(error) || error == Errors.OAuth.ServerError)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error()
            {
                Code = Errors.OAuth.ServerError, 
                Description = "Server error."
            });
        }
        else
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error()
            {
                Code = error, 
                Description = "Invalid token."
            });
        }
    }
}