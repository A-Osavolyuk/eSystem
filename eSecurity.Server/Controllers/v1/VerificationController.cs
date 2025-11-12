using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Verification.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
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

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Resend code")]
    [EndpointDescription("Resend code")]
    [ProducesResponseType(200)]
    [HttpPost("code/resend")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ResendCodeAsync([FromBody] ResendCodeRequest request)
    {
        var result = await _sender.Send(new ResendCodeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify code")]
    [EndpointDescription("verify code")]
    [ProducesResponseType(200)]
    [HttpPost("code/verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyCodeAsync([FromBody] VerifyCodeRequest request)
    {
        var result = await _sender.Send(new VerifyCodeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify authenticator code")]
    [EndpointDescription("verify authenticator code")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyAuthenticatorCodeAsync([FromBody] VerifyAuthenticatorCodeRequest request)
    {
        var result = await _sender.Send(new VerifyAuthenticatorCodeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify passkey")]
    [EndpointDescription("verify passkey")]
    [ProducesResponseType(200)]
    [HttpPost("passkey/verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyPasskeyAsync([FromBody] VerifyPasskeyRequest request)
    {
        var result = await _sender.Send(new VerifyPasskeyCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}