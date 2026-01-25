using System.Text.Encodings.Web;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers.Basic;

public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public class BasicAuthenticationHandler(
    IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    IBasicAuthenticationStrategyResolver strategyResolver,
    UrlEncoder encoder) : AuthenticationHandler<BasicAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IBasicAuthenticationStrategyResolver _strategyResolver = strategyResolver;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var strategy = _strategyResolver.Resolve(Context);
            return await strategy.ExecuteAsync(Context);
        }
        catch (InvalidOperationException)
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
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
                Description = "Invalid client."
            });
        }
    }
}