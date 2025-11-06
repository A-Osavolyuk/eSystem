using eSecurity.Common.Responses;
using eSecurity.Features.LinkedAccounts.Commands;
using eSecurity.Security.Authentication.Schemes;
using eSecurity.Security.Authentication.SignIn;

namespace eSecurity.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public class OAuthController(ISender sender, ISignInManager signInManager) : ControllerBase
{
    private readonly ISender sender = sender;
    private readonly ISignInManager signInManager = signInManager;
    
    [EndpointSummary("OAuth login")]
    [EndpointDescription("OAuth login")]
    [ProducesResponseType(200)]
    [HttpGet("login/{type}")]
    public async ValueTask<IActionResult> OAuthLoginAsync(string type, string returnUri, string fallbackUri)
    {
        var result = await sender.Send(new LinkedAccountLoginCommand(type, returnUri, fallbackUri));

        return result.Match<IActionResult>(
            s =>
            {
                var response = s.Value! as OAuthLoginResponse;
                return Challenge(response!.AuthenticationProperties, response.Provider);
            },
            f => Redirect(f.Value!.ToString()!));
    }

    [EndpointSummary("Handle OAuth login")]
    [EndpointDescription("Handles OAuth login")]
    [ProducesResponseType(200)]
    [HttpGet("handle")]
    public async ValueTask<IActionResult> HandleOAuthLoginAsync(string? remoteError = null,
        string? returnUri = null)
    {
        var authenticationResult = await signInManager.AuthenticateAsync(
            AuthenticationDefaults.AuthenticationScheme);
        
        var result = await sender.Send(new HandleLoginCommand(authenticationResult, remoteError, returnUri));
        return result.Match(
            s => Redirect(s.Message),
            f => Redirect(f.Value!.ToString()!));
    }
}