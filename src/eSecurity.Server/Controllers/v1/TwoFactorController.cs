using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.TwoFactor.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TwoFactorController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [EndpointSummary("Enable 2FA")]
    [EndpointDescription("Enable 2FA")]
    [ProducesResponseType(200)]
    [HttpPost("enable")]
    public async ValueTask<IActionResult> EnableAsync()
    {
        var result = await _sender.Send(new EnableTwoFactorCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Disable 2FA")]
    [EndpointDescription("Disable 2FA")]
    [ProducesResponseType(200)]
    [HttpPost("disable")]
    public async ValueTask<IActionResult> DisableAsync()
    {
        var result = await _sender.Send(new DisableTwoFactorCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Prefer 2FA method")]
    [EndpointDescription("Prefer 2FA method")]
    [ProducesResponseType(200)]
    [HttpPost("prefer")]
    public async ValueTask<IActionResult> PreferAsync(
        [FromBody] PreferTwoFactorMethodRequest request)
    {
        var result = await _sender.Send(new PreferTwoFactorMethodCommand(request));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Generate QR code")]
    [EndpointDescription("Generate QR code")]
    [ProducesResponseType(200)]
    [HttpPost("qr-code/generate")]
    public async ValueTask<IActionResult> GenerateQrCodeAsync()
    {
        var result = await _sender.Send(new GenerateQrCodeCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Regenerate QR code")]
    [EndpointDescription("Regenerate QR code")]
    [ProducesResponseType(200)]
    [HttpPost("qr-code/regenerate")]
    public async ValueTask<IActionResult> RegenerateQrCodeAsync()
    {
        var result = await _sender.Send(new RegenerateQrCodeCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Generate recovery codes")]
    [EndpointDescription("Generate recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-codes/generate")]
    public async ValueTask<IActionResult> GenerateRecoveryCodesAsync()
    {
        var result = await _sender.Send(new GenerateRecoveryCodesCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Load recovery codes")]
    [EndpointDescription("Load recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-codes/load")]
    public async ValueTask<IActionResult> LoadRecoveryCodesAsync()
    {
        var result = await _sender.Send(new LoadRecoveryCodesCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Verify authenticator")]
    [EndpointDescription("Verify authenticator")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/verify")]
    public async ValueTask<IActionResult> VerifyAuthenticatorAsync(
        [FromBody] VerifyAuthenticatorRequest request)
    {
        var result = await _sender.Send(new VerifyAuthenticatorCommand(request));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Reconfigure authenticator")]
    [EndpointDescription("Reconfigure authenticator")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/reconfigure")]
    public async ValueTask<IActionResult> ReconfigureAuthenticatorAsync(
        [FromBody] ReconfigureAuthenticatorRequest request)
    {
        var result = await _sender.Send(new ReconfigureAuthenticatorCommand(request));
        return HttpContext.HandleResult(result);
    }
}