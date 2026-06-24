using eSecurity.Idp.Features.LinkedAccounts;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LinkedAccountController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("Disconnect linked account")]
    [EndpointDescription("Disconnect linked account")]
    [ProducesResponseType(200)]
    [HttpPost("disconnect")]
    public async ValueTask<IActionResult> DisconnectAsync([FromBody] DisconnectLinkedAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
}