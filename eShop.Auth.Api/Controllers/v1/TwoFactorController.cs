using eShop.Auth.Api.Features.TwoFactor.Commands;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class TwoFactorController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Authenticator app sign-in")]
    [EndpointDescription("Authenticator app sign-in")]
    [ProducesResponseType(200)]
    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> AuthenticatorAppSignInAsync([FromBody] AuthenticatorSignInRequest request)
    {
        var result = await sender.Send(new AuthenticatorSignInCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Generate recovery codes")]
    [EndpointDescription("Generate recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-code/generate")]
    [ValidationFilter]
    public async ValueTask<IActionResult> GenerateRecoveryCodes([FromBody] GenerateRecoveryCodesRequest request)
    {
        var result = await sender.Send(new GenerateRecoveryCodesCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Load recovery codes")]
    [EndpointDescription("Load recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-code/load")]
    [ValidationFilter]
    public async ValueTask<IActionResult> LoadRecoveryCodesAsync([FromBody] LoadRecoveryCodesRequest request)
    {
        var result = await sender.Send(new LoadRecoveryCodesCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Revoke recovery codes")]
    [EndpointDescription("Revoke recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-code/revoke")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RevokeRecoveryCodes([FromBody] RevokeRecoveryCodesRequest request)
    {
        var result = await sender.Send(new RevokeRecoveryCodesCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Generate QR code")]
    [EndpointDescription("Generate QR code")]
    [ProducesResponseType(200)]
    [HttpPost("qr-code/generate")]
    [Authorize]
    public async ValueTask<IActionResult> GenerateQrCodeAsync(
        [FromBody] GenerateQrCodeRequest request)
    {
        var result = await sender.Send(new GenerateQrCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Enable 2FA")]
    [EndpointDescription("Enable 2FA")]
    [ProducesResponseType(200)]
    [HttpPost("enable")]
    [Authorize]
    public async ValueTask<IActionResult> EnableTwoFactorAsync([FromBody] EnableTwoFactorRequest request)
    {
        var result = await sender.Send(new EnableCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Disable 2FA")]
    [EndpointDescription("Disable 2FA")]
    [ProducesResponseType(200)]
    [HttpPost("disable")]
    [Authorize]
    public async ValueTask<IActionResult> DisableTwoFactorAsync([FromBody] DisableTwoFactorRequest request)
    {
        var result = await sender.Send(new DisableCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Prefer 2FA method")]
    [EndpointDescription("Prefer 2FA method")]
    [ProducesResponseType(200)]
    [HttpPost("prefer")]
    [Authorize]
    public async ValueTask<IActionResult> PreferMethodAsync([FromBody] PreferTwoFactorMethodRequest request)
    {
        var result = await sender.Send(new PreferMethodCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}