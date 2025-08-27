using eShop.Auth.Api.Features.Devices.Commands;
using eShop.Auth.Api.Features.Devices.Queries;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class DeviceController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get device")]
    [EndpointDescription("Get device")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async ValueTask<IActionResult> GetDeviceAsync(Guid id)
    {
        var result = await sender.Send(new GetDeviceQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Trust device")]
    [EndpointDescription("Trust device")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("trust")]
    [ValidationFilter]
    public async ValueTask<IActionResult> TrustDeviceAsync([FromBody] TrustDeviceRequest request)
    {
        var result = await sender.Send(new TrustDeviceCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Block device")]
    [EndpointDescription("Block device")]
    [ProducesResponseType(200)]
    [HttpPost("block")]
    [ValidationFilter]
    public async ValueTask<IActionResult> BlockDeviceAsync([FromBody] BlockDeviceRequest request)
    {
        var result = await sender.Send(new BlockDeviceCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm device block")]
    [EndpointDescription("Confirm device block")]
    [ProducesResponseType(200)]
    [HttpPost("confirm-block")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ConfirmDeviceBlockAsync([FromBody] ConfirmBlockDeviceRequest request)
    {
        var result = await sender.Send(new ConfirmBlockDeviceCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Unblock device")]
    [EndpointDescription("Unblock device")]
    [ProducesResponseType(200)]
    [HttpPost("unblock")]
    [ValidationFilter]
    public async ValueTask<IActionResult> UnblockDeviceAsync([FromBody] UnblockDeviceRequest request)
    {
        var result = await sender.Send(new UnblockDeviceCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm device unblock")]
    [EndpointDescription("Confirm device unblock")]
    [ProducesResponseType(200)]
    [HttpPost("confirm-unblock")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ConfirmDeviceUnblockAsync([FromBody] ConfirmUnblockDeviceRequest request)
    {
        var result = await sender.Send(new ConfirmUnblockDeviceCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify device")]
    [EndpointDescription("Verify device")]
    [ProducesResponseType(200)]
    [HttpPost("verify")]
    [ValidationFilter]
    public async ValueTask<IActionResult> VerifyDeviceAsync([FromBody] VerifyDeviceRequest request)
    {
        var result = await sender.Send(new VerifyDeviceCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm device verification")]
    [EndpointDescription("Confirm device verification")]
    [ProducesResponseType(200)]
    [HttpPost("confirm-verification")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ConfirmDeviceVerificationAsync([FromBody] ConfirmVerifyDeviceRequest request)
    {
        var result = await sender.Send(new ConfirmVerifyDeviceCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}