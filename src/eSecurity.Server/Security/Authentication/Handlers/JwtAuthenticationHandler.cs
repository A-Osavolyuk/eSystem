using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using eSecurity.Server.Security.Authorization.Constants;
using eSecurity.Server.Security.Authorization.OAuth.Token.Validation;
using eSystem.Core.Http.Constants;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers;

public sealed class JwtAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public sealed class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationSchemeOptions> options, 
    ILoggerFactory logger, 
    UrlEncoder encoder,
    ITokenValidationProvider validationProvider
    ) : AuthenticationHandler<JwtAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly ITokenValidationProvider _validationProvider = validationProvider;
    private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(header) || !header.StartsWith($"{AuthenticationTypes.Bearer} "))
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidToken;
            return AuthenticateResult.Fail("Invalid token.");
        }
        
        var token = header.Split(" ").Last();
        var tokenKind = _handler.CanReadToken(token) ? TokenKind.Jwt : TokenKind.Opaque;
        var validator = _validationProvider.CreateValidator(tokenKind);
        var result = await validator.ValidateAsync(token);
        
        if (!result.IsValid || result.ClaimsPrincipal is null)
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidToken;
            return AuthenticateResult.Fail("Invalid token");
        }
        
        var ticket = new AuthenticationTicket(result.ClaimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var error = Context.Items["error"]?.ToString();

        if (string.IsNullOrEmpty(error) || error == ErrorTypes.OAuth.ServerError)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error
            {
                Code = ErrorTypes.OAuth.ServerError, 
                Description = "Server error."
            });
        }
        else
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error
            {
                Code = error, 
                Description = "Invalid token."
            });
        }
    }
}