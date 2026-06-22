using eSecurity.Idp.Common.Responses;
using eSecurity.Idp.Features.OAuth;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using RedirectResult = eSystem.Core.Primitives.RedirectResult;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class OAuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("OAuth login")]
    [EndpointDescription("OAuth login")]
    [ProducesResponseType(200)]
    [HttpGet("login")]
    public async ValueTask<IActionResult> OAuthLoginAsync(string provider, string returnUri, string state)
    {
        var result = await _mediator.Send(new OAuthLoginCommand(provider, returnUri, state));

        if (result is ValueResult objectResult)
        {
            var response = objectResult.GetValue<OAuthLoginResponse>();
            return Challenge(response.AuthenticationProperties, response.Provider);
        }

        if (result is RedirectResult redirectResult)
            return Redirect(redirectResult.Uri);

        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Handle OAuth login")]
    [EndpointDescription("Handles OAuth login")]
    [ProducesResponseType(200)]
    [HttpGet("handle")]
    public async ValueTask<IActionResult> HandleOAuthLoginAsync(string returnUri, string? remoteError = null)
    {
        var result = await _mediator.Send(new HandleLoginCommand(remoteError, returnUri));
        return HttpContext.HandleResult(result);
    }
}