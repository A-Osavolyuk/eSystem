using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Devices.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class DeviceController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Trust device")]
    [EndpointDescription("Trust device")]
    [ProducesResponseType(200)]
    [HttpPost("trust")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> TrustAsync([FromBody] TrustDeviceRequest request)
    {
        var result = await sender.Send(new TrustDeviceCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Block device")]
    [EndpointDescription("Block device")]
    [ProducesResponseType(200)]
    [HttpPost("block")]
    [Authorize]
    public async ValueTask<IActionResult> BlockAsync([FromBody] BlockDeviceRequest request)
    {
        var result = await sender.Send(new BlockDeviceCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Unblock device")]
    [EndpointDescription("Unblock device")]
    [ProducesResponseType(200)]
    [HttpPost("unblock")]
    [Authorize]
    public async ValueTask<IActionResult> UnblockAsync([FromBody] UnblockDeviceRequest request)
    {
        var result = await sender.Send(new UnblockDeviceCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify device")]
    [EndpointDescription("Verify device")]
    [ProducesResponseType(200)]
    [HttpPost("verify")]
    [Authorize]
    public async ValueTask<IActionResult> VerifyAsync([FromBody] VerifyDeviceRequest request)
    {
        var result = await sender.Send(new VerifyDeviceCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}