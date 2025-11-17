using eSecurity.Core.Common.Requests;
using eSecurity.Server.Common.Responses;
using eSecurity.Server.Features.OAuth;
using eSecurity.Server.Security.Authentication.SignIn;
using eSecurity.Server.Security.Authorization.OAuth.Schemes;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Server.Controllers.v1;

[Route("v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public class OAuthController(ISender sender, ISignInManager signInManager) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ISignInManager _signInManager = signInManager;
    
    [EndpointSummary("OAuth login")]
    [EndpointDescription("OAuth login")]
    [ProducesResponseType(200)]
    [HttpGet("login/{type}")]
    public async ValueTask<IActionResult> OAuthLoginAsync(string type, string returnUri, string fallbackUri)
    {
        var result = await _sender.Send(new OAuthLoginCommand(type, returnUri, fallbackUri));

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
        var authenticationResult = await _signInManager.AuthenticateAsync(
            AuthenticationDefaults.AuthenticationScheme);
        
        var result = await _sender.Send(new HandleLoginCommand(authenticationResult, remoteError, returnUri));
        return result.Match(
            s => Redirect(s.Value!.ToString()!),
            f => Redirect(f.Value!.ToString()!));
    }
    
    [EndpointSummary("Load session")]
    [EndpointDescription("Load session")]
    [ProducesResponseType(200)]
    [HttpPost("load")]
    public async ValueTask<IActionResult> LoadAsync([FromBody] LoadOAuthSessionRequest request)
    {
        var result = await _sender.Send(new LoadOAuthSessionCommand(request));
        return ResultHandler.Handle(result);
    }
}