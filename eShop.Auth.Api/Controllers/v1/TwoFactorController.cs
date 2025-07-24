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
    
    [EndpointSummary("Change two-factor state")]
    [EndpointDescription("Changes two-factor state")]
    [ProducesResponseType(200)]
    [HttpPost("change-state")]
    public async ValueTask<ActionResult<Response>> ChangeTwoFactorAuthentication(
        [FromBody] ChangeTwoFactorStateRequest request)
    {
        var result = await sender.Send(new ChangeTwoFactorStateCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Send two-factor token")]
    [EndpointDescription("Send two-factor token")]
    [ProducesResponseType(200)]
    [HttpPost("send-token")]
    [AllowAnonymous]
    public async ValueTask<ActionResult<Response>> SendTokenAsync(
        [FromBody] SendTwoFactorTokenRequest request)
    {
        var result = await sender.Send(new SendTwoFactorTokenCommand(request));

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