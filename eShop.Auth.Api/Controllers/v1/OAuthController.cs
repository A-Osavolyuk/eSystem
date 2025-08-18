using eShop.Auth.Api.Features.OAuth.Commands;
using eShop.Auth.Api.Security.Schemes;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

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
    [HttpGet("login/{provider}")]
    public async ValueTask<IActionResult> OAuthLoginAsync(string provider, string returnUri, string fallbackUri)
    {
        var result = await sender.Send(new OAuthLoginCommand(provider, returnUri, fallbackUri));

        return result.Match(
            s =>
            {
                var response = s.Value! as OAuthLoginResponse;
                return Challenge(response!.AuthenticationProperties, response.Provider);
            },
            ErrorHandler.Handle);
    }

    [EndpointSummary("Handle OAuth login")]
    [EndpointDescription("Handles OAuth login")]
    [ProducesResponseType(200)]
    [HttpGet("handle")]
    public async ValueTask<IActionResult> HandleOAuthLoginAsync(string? remoteError = null,
        string? returnUri = null)
    {
        var authenticationResult = await signInManager
            .AuthenticateAsync(HttpContext, ExternalAuthenticationDefaults.AuthenticationScheme);
        
        var result = await sender.Send(new HandleOAuthLoginCommand(HttpContext, authenticationResult, remoteError, returnUri));
        return result.Match(s => Redirect(s.Message), ErrorHandler.Handle);
    }
    
    [EndpointSummary("Load OAuth session")]
    [EndpointDescription("Load OAuth session")]
    [ProducesResponseType(200)]
    [HttpPost("load")]
    public async ValueTask<IActionResult> LoadOauthSessionAsync([FromBody] LoadOAuthSessionRequest request)
    {
        var result = await sender.Send(new LoadOAuthSessionCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}