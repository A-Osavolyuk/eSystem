using eSystem.Auth.Api.Features.LinkedAccounts.Commands;
using eSystem.Auth.Api.Security.Authentication.Schemes;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;

namespace eSystem.Auth.Api.Controllers.v1;

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
            ExternalAuthenticationDefaults.AuthenticationScheme);
        
        var result = await sender.Send(new HandleLoginCommand(authenticationResult, remoteError, returnUri));
        return result.Match(
            s => Redirect(s.Message),
            f => Redirect(f.Value!.ToString()!));
    }
    
    [EndpointSummary("Load OAuth session")]
    [EndpointDescription("Load OAuth session")]
    [ProducesResponseType(200)]
    [HttpPost("load")]
    public async ValueTask<IActionResult> LoadSessionAsync([FromBody] LoadOAuthSessionRequest request)
    {
        var result = await sender.Send(new LoadSessionCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Disconnect related account")]
    [EndpointDescription("Disconnect related account")]
    [ProducesResponseType(200)]
    [HttpPost("disconnect")]
    public async ValueTask<IActionResult> DisconnectAsync([FromBody] DisconnectLinkedAccountRequest request)
    {
        var result = await sender.Send(new DisconnectLinkedAccountCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}