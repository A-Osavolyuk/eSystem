using eSecurity.Idp.Features.DeviceCode.Commands;
using eSecurity.Idp.Features.DeviceCode.Queries;
using eSecurity.Core.Requests;
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
    public async ValueTask<IActionResult> CheckDeviceCode([FromBody] CheckDeviceCodeRequest request)
    {
        var result = await _mediator.Send(new CheckDeviceCodeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Device code decision")]
    [EndpointDescription("Device code decision")]
    [ProducesResponseType(200)]
    [HttpPost("device-code/decision")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> DecisionAsync([FromBody] DeviceCodeDecisionRequest request)
    {
        var result = await _mediator.Send(new DeviceCodeDecisionCommand(request));
        return HttpContext.HandleResult(result);
    }
}