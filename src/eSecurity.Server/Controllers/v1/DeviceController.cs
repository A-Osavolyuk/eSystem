using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Devices.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class DeviceController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Block device")]
    [EndpointDescription("Block device")]
    [ProducesResponseType(200)]
    [HttpPost("block")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> BlockAsync([FromBody] BlockDeviceRequest request)
    {
        var result = await _sender.Send(new BlockDeviceCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Unblock device")]
    [EndpointDescription("Unblock device")]
    [ProducesResponseType(200)]
    [HttpPost("unblock")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> UnblockAsync([FromBody] UnblockDeviceRequest request)
    {
        var result = await _sender.Send(new UnblockDeviceCommand(request));
        return HttpContext.HandleResult(result);
    }
}