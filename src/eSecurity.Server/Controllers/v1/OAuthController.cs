using eSecurity.Server.Common.Responses;
using eSecurity.Server.Features.OAuth.Commands;
using eSecurity.Server.Features.OAuth.Queries;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class OAuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
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
        var result = await _sender.Send(new HandleLoginCommand(remoteError, returnUri));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get OAuth session")]
    [EndpointDescription("Get OAuth session")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}")]
    public async ValueTask<IActionResult> GetSessionAsync(Guid id)
    {
        var result = await _sender.Send(new GetOAuthSessionQuery(id));
        return HttpContext.HandleResult(result);
    }
}