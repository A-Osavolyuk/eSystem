using eShop.Auth.Api.Features.Users.Commands;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class TwoFactorController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Login two-factor")]
    [EndpointDescription("Login with two-factor")]
    [ProducesResponseType(200)]
    [HttpPost("login")]
    [AllowAnonymous]
    public async ValueTask<ActionResult<Response>> LoginWithTwoFactorAuthenticationCode(
        [FromBody] TwoFactorLoginRequest request)
    {
        var result =
            await sender.Send(new TwoFactorLoginCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Send 2FA code")]
    [EndpointDescription("Send 2FA code")]
    [ProducesResponseType(200)]
    [HttpPost("send-code")]
    [AllowAnonymous]
    public async ValueTask<ActionResult<Response>> SendTokenAsync(
        [FromBody] SendTwoFactorCodeRequest request)
    {
        var result = await sender.Send(new SendTwoFactorCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Generate QR code")]
    [EndpointDescription("Generate QR code")]
    [ProducesResponseType(200)]
    [HttpPost("generate-qr-code")]
    public async ValueTask<ActionResult<Response>> GenerateQrCodeAsync(
        [FromBody] GenerateQrCodeRequest request)
    {
        var result = await sender.Send(new GenerateQrCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}