using eSecurity.Server.Common.Responses;
using eSecurity.Server.Features.OAuth;
using eSecurity.Server.Security.Authentication.SignIn;
using eSystem.Core.Common.Http.Constants;
using eSystem.Core.Security.Authentication.Schemes;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class OAuthController(ISender sender, ISignInManager signInManager) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ISignInManager _signInManager = signInManager;
    
    [EndpointSummary("OAuth login")]
    [EndpointDescription("OAuth login")]
    [ProducesResponseType(200)]
    [HttpGet("login")]
    public async ValueTask<IActionResult> OAuthLoginAsync(string provider, string returnUri, string state)
    {
        var result = await _sender.Send(new OAuthLoginCommand(provider, returnUri, state));

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
    public async ValueTask<IActionResult> HandleOAuthLoginAsync(string returnUri, string? remoteError = null)
    {
        var authenticationResult = await _signInManager.AuthenticateAsync(
            ExternalAuthenticationDefaults.AuthenticationScheme);
        
        var result = await _sender.Send(new HandleLoginCommand(authenticationResult, remoteError, returnUri));
        return Redirect(result.Value!.ToString()!);
    }
}