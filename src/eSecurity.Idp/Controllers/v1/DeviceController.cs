using eSecurity.Core.Requests;
using eSecurity.Idp.Features.DeviceCode;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class DeviceController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("Get device code info")]
    [EndpointDescription("Get device code info")]
    [ProducesResponseType(200)]
    [HttpGet("device-code/{userCode}")]
    public async ValueTask<IActionResult> GetDeviceCodeInfo(string userCode)
    {
        var result = await _mediator.Send(new GetDeviceCodeInfoQuery(userCode));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check device code")]
    [EndpointDescription("Check device code")]
    [ProducesResponseType(200)]
    [HttpPost("device-code/check")]
    public async ValueTask<IActionResult> CheckDeviceCode([FromBody] CheckDeviceCodeCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Device code decision")]
    [EndpointDescription("Device code decision")]
    [ProducesResponseType(200)]
    [HttpPost("device-code/decision")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> DecisionAsync([FromBody] DeviceCodeDecisionCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
}