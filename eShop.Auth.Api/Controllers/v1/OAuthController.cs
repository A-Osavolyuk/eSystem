using eShop.Auth.Api.Features.OAuth.Queries;
using eShop.Auth.Api.Security.Schemes;
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
    [AllowAnonymous]
    [HttpGet("oauth-login/{provider}")]
    public async ValueTask<ActionResult<Response>> OAuthLoginAsync(string provider, string? returnUri = null)
    {
        var result = await sender.Send(new OAuthLoginQuery(provider, returnUri));

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
    [AllowAnonymous]
    [HttpGet("handle-oauth-login")]
    public async ValueTask<ActionResult<Response>> HandleOAuthLoginAsync(string? remoteError = null,
        string? returnUri = null)
    {
        var principal = await signInManager.AuthenticateAsync(HttpContext, ExternalAuthenticationDefaults.AuthenticationScheme);
        
        var result = await sender.Send(new HandleOAuthLoginQuery(principal, remoteError, returnUri));
        return result.Match(s => Redirect(Convert.ToString(s.Message!)!), ErrorHandler.Handle);
    }
}