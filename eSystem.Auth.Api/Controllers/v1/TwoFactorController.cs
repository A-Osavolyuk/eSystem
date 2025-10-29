using eSystem.Auth.Api.Features.TwoFactor.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;

namespace eSystem.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class TwoFactorController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Generate recovery codes")]
    [EndpointDescription("Generate recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-code/generate")]
    [ValidationFilter]
    public async ValueTask<IActionResult> GenerateRecoveryCodesAsync([FromBody] GenerateRecoveryCodesRequest request)
    {
        var result = await sender.Send(new GenerateRecoveryCodesCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
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
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Revoke recovery codes")]
    [EndpointDescription("Revoke recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-code/revoke")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RevokeRecoveryCodesAsync([FromBody] RevokeRecoveryCodesRequest request)
    {
        var result = await sender.Send(new RevokeRecoveryCodesCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify recovery code")]
    [EndpointDescription("Verify recovery code")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-code/verify")]
    [ValidationFilter]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyRecoveryCodeAsync([FromBody] VerifyRecoveryCodeRequest codeRequest)
    {
        var result = await sender.Send(new VerifyRecoveryCodeCommand(codeRequest));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
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
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithResult(s.Value!)
                .WithMessage(s.Message)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Regenerate QR code")]
    [EndpointDescription("Regenerate QR code")]
    [ProducesResponseType(200)]
    [HttpPost("qr-code/regenerate")]
    [Authorize]
    public async ValueTask<IActionResult> RegenerateQrCodeAsync(
        [FromBody] RegenerateQrCodeRequest request)
    {
        var result = await sender.Send(new RegenerateQrCodeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithResult(s.Value!)
                .WithMessage(s.Message)
                .Build()),
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
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithResult(s.Value!)
                .WithMessage(s.Message)
                .Build()),
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
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithResult(s.Value!)
                .WithMessage(s.Message)
                .Build()),
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
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithResult(s.Value!)
                .WithMessage(s.Message)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Reconfigure authenticator app")]
    [EndpointDescription("Reconfigure authenticator app")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/reconfigure")]
    [Authorize]
    public async ValueTask<IActionResult> ReconfigureAuthenticatorAsync(
        [FromBody] ReconfigureAuthenticatorRequest request)
    {
        var result = await sender.Send(new ReconfigureAuthenticatorCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithResult(s.Value!)
                .WithMessage(s.Message)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify authenticator app")]
    [EndpointDescription("Verify authenticator app")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/verify")]
    [Authorize]
    public async ValueTask<IActionResult> VerifyAuthenticatorAsync(
        [FromBody] VerifyAuthenticatorRequest request)
    {
        var result = await sender.Send(new VerifyAuthenticatorCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithResult(s.Value!)
                .WithMessage(s.Message)
                .Build()),
            ErrorHandler.Handle);
    }
}