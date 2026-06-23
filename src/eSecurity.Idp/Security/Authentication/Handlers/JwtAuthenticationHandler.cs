using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using eSecurity.Idp.Security.Authorization.Constants;
using eSecurity.Idp.Security.Authorization.Token.Validation;
using eSystem.Core.Enums;
using eSystem.Core.Http.Constants;
using eSystem.Core.Primitives;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Idp.Security.Authentication.Handlers;

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
            Context.Items["error"] = ErrorCode.InvalidToken;
            return AuthenticateResult.Fail("Invalid token.");
        }
        
        var token = header.Split(" ").Last();
        var tokenKind = _handler.CanReadToken(token) ? TokenKind.Jwt : TokenKind.Opaque;
        var validator = _validationProvider.CreateValidator(tokenKind);
        var result = await validator.ValidateAsync(token);
        
        if (!result.IsValid || result.ClaimsPrincipal is null)
        {
            Context.Items["error"] = ErrorCode.InvalidToken;
            return AuthenticateResult.Fail("Invalid token");
        }
        
        var ticket = new AuthenticationTicket(result.ClaimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var errorString = Context.Items["error"]?.ToString() ?? string.Empty;
        var error = EnumHelper.ParseFromString<ErrorCode>(errorString);
        if (error is null || error.Value is ErrorCode.ServerError)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error
            {
                Code = ErrorCode.ServerError, 
                Description = "Server error."
            });
        }
        else
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error
            {
                Code = error.Value, 
                Description = "Invalid token."
            });
        }
    }
}