using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Devices.Commands;
using eSystem.Core.Common.Http.Constants;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class DeviceController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Trust device")]
    [EndpointDescription("Trust device")]
    [ProducesResponseType(200)]
    [HttpPost("trust")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> TrustAsync([FromBody] TrustDeviceRequest request)
    {
        var result = await _sender.Send(new TrustDeviceCommand(request));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Block device")]
    [EndpointDescription("Block device")]
    [ProducesResponseType(200)]
    [HttpPost("block")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> BlockAsync([FromBody] BlockDeviceRequest request)
    {
        var result = await _sender.Send(new BlockDeviceCommand(request));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Unblock device")]
    [EndpointDescription("Unblock device")]
    [ProducesResponseType(200)]
    [HttpPost("unblock")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> UnblockAsync([FromBody] UnblockDeviceRequest request)
    {
        var result = await _sender.Send(new UnblockDeviceCommand(request));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Verify device")]
    [EndpointDescription("Verify device")]
    [ProducesResponseType(200)]
    [HttpPost("verify")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> VerifyAsync([FromBody] VerifyDeviceRequest request)
    {
        var result = await _sender.Send(new VerifyDeviceCommand(request));
        return ResultHandler.Handle(result);
    }
}