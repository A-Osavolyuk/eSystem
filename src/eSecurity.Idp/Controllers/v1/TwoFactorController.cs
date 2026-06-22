using eSecurity.Core.Requests;
using eSecurity.Idp.Features.TwoFactor;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TwoFactorController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [EndpointSummary("Enable 2FA")]
    [EndpointDescription("Enable 2FA")]
    [ProducesResponseType(200)]
    [HttpPost("enable")]
    public async ValueTask<IActionResult> EnableAsync([FromBody] EnableTwoFactorCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Disable 2FA")]
    [EndpointDescription("Disable 2FA")]
    [ProducesResponseType(200)]
    [HttpPost("disable")]
    public async ValueTask<IActionResult> DisableAsync([FromBody] DisableTwoFactorCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Prefer 2FA method")]
    [EndpointDescription("Prefer 2FA method")]
    [ProducesResponseType(200)]
    [HttpPost("prefer")]
    public async ValueTask<IActionResult> PreferAsync([FromBody] PreferTwoFactorMethodCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Generate QR code")]
    [EndpointDescription("Generate QR code")]
    [ProducesResponseType(200)]
    [HttpPost("qr-code/generate")]
    public async ValueTask<IActionResult> GenerateQrCodeAsync()
    {
        var result = await _mediator.Send(new GenerateQrCodeCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Regenerate QR code")]
    [EndpointDescription("Regenerate QR code")]
    [ProducesResponseType(200)]
    [HttpPost("qr-code/regenerate")]
    public async ValueTask<IActionResult> RegenerateQrCodeAsync()
    {
        var result = await _mediator.Send(new RegenerateQrCodeCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Generate recovery codes")]
    [EndpointDescription("Generate recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-codes/generate")]
    public async ValueTask<IActionResult> GenerateRecoveryCodesAsync()
    {
        var result = await _mediator.Send(new GenerateRecoveryCodesCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Load recovery codes")]
    [EndpointDescription("Load recovery codes")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-codes/load")]
    public async ValueTask<IActionResult> LoadRecoveryCodesAsync()
    {
        var result = await _mediator.Send(new LoadRecoveryCodesCommand());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Verify authenticator")]
    [EndpointDescription("Verify authenticator")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/verify")]
    public async ValueTask<IActionResult> VerifyAuthenticatorAsync([FromBody] VerifyAuthenticatorCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Reconfigure authenticator")]
    [EndpointDescription("Reconfigure authenticator")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/reconfigure")]
    public async ValueTask<IActionResult> ReconfigureAuthenticatorAsync(
        [FromBody] ReconfigureAuthenticatorCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
}