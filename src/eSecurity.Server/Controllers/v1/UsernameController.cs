using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Username.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class UsernameController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Set username")]
    [EndpointDescription("Set username")]
    [ProducesResponseType(200)]
    [HttpPost("set")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> SetAsync([FromBody] SetUsernameRequest request)
    {
        var result = await _sender.Send(new SetUsernameCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check username")]
    [EndpointDescription("Check username")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckUsernameRequest request)
    {
        var result = await _sender.Send(new CheckUsernameCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Change username")]
    [EndpointDescription("Change username")]
    [ProducesResponseType(200)]
    [HttpPut("change")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> EnableAsync([FromBody] ChangeUsernameRequest request)
    {
        var result = await _sender.Send(new ChangeUsernameCommand(request));
        return HttpContext.HandleResult(result);
    }
}