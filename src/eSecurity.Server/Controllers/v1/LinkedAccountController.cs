using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.LinkedAccounts.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class LinkedAccountController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Disconnect linked account")]
    [EndpointDescription("Disconnect linked account")]
    [ProducesResponseType(200)]
    [HttpPost("disconnect")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> DisconnectAsync([FromBody] DisconnectLinkedAccountRequest request)
    {
        var result = await _sender.Send(new DisconnectLinkedAccountCommand(request));
        return HttpContext.HandleResult(result);
    }
}