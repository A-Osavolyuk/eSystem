using eSecurity.Core.Requests;
using eSecurity.Idp.Features.Connect;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class ConsentController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("Check consents")]
    [EndpointDescription("Check consents")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckConsentCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Grant consents")]
    [EndpointDescription("Grant consents")]
    [ProducesResponseType(200)]
    [HttpPost("grant")]
    public async ValueTask<IActionResult> GrantAsync([FromBody] GrantConsentCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
}