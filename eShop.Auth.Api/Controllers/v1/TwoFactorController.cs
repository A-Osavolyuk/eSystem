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
    [HttpPost("code/send")]
    [AllowAnonymous]
    public async ValueTask<ActionResult<Response>> SendCodeAsync(
        [FromBody] SendTwoFactorCodeRequest request)
    {
        var result = await sender.Send(new SendTwoFactorCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify 2FA code")]
    [EndpointDescription("Verify 2FA code")]
    [ProducesResponseType(200)]
    [HttpPost("code/verify")]
    [Authorize]
    public async ValueTask<ActionResult<Response>> VerifyCodeAsync(
        [FromBody] VerifyTwoFactorCodeRequest request)
    {
        var result = await sender.Send(new VerifyTwoFactorCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Generate recovery codes")]
    [EndpointDescription("Generate recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-code/generate")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> GenerateRecoveryCodes([FromBody] GenerateRecoveryCodesRequest request)
    {
        var result = await sender.Send(new GenerateRecoveryCodesCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}