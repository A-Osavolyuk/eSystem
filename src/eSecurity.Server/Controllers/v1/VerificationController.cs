using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Verification.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class VerificationController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Send code")]
    [EndpointDescription("Send code")]
    [ProducesResponseType(200)]
    [HttpPost("code/send")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> SendCodeAsync([FromBody] SendCodeRequest request)
    {
        var result = await _sender.Send(new SendCodeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Resend code")]
    [EndpointDescription("Resend code")]
    [ProducesResponseType(200)]
    [HttpPost("code/resend")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ResendCodeAsync([FromBody] ResendCodeRequest request)
    {
        var result = await _sender.Send(new ResendCodeCommand(request));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Verify code")]
    [EndpointDescription("verify code")]
    [ProducesResponseType(200)]
    [HttpPost("code/verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyCodeAsync([FromBody] VerifyCodeRequest request)
    {
        var result = await _sender.Send(new VerifyCodeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verify code")]
    [EndpointDescription("verify code")]
    [ProducesResponseType(200)]
    [HttpPost("recovery-code/verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyRecoveryCodeAsync([FromBody] VerifyRecoveryCodeRequest request)
    {
        var result = await _sender.Send(new VerifyRecoveryCodeCommand(request));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Verify authenticator code")]
    [EndpointDescription("verify authenticator code")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyAuthenticatorCodeAsync([FromBody] VerifyAuthenticatorCodeRequest request)
    {
        var result = await _sender.Send(new VerifyAuthenticatorCodeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verify passkey")]
    [EndpointDescription("verify passkey")]
    [ProducesResponseType(200)]
    [HttpPost("passkey/verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyPasskeyAsync([FromBody] VerifyPasskeyRequest request)
    {
        var result = await _sender.Send(new VerifyPasskeyCommand(request));
        return HttpContext.HandleResult(result);
    }
}