using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Connect.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class ConsentController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Check consents")]
    [EndpointDescription("Check consents")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckConsentRequest request)
    {
        var result = await _sender.Send(new CheckConsentCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Grant consents")]
    [EndpointDescription("Grant consents")]
    [ProducesResponseType(200)]
    [HttpPost("grant")]
    public async ValueTask<IActionResult> GrantAsync([FromBody] GrantConsentRequest request)
    {
        var result = await _sender.Send(new GrantConsentCommand(request));
        return HttpContext.HandleResult(result);
    }
}