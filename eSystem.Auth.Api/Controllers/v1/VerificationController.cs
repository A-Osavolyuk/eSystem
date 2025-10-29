using eSystem.Auth.Api.Features.Verification.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[AllowAnonymous]
public class VerificationController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Send code")]
    [EndpointDescription("Sends code")]
    [ProducesResponseType(200)]
    [HttpPost("code/send")]
    public async ValueTask<IActionResult> SendCodeAsync([FromBody] SendCodeRequest request)
    {
        var result = await sender.Send(new SendCodeCommand(request));

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
    public async ValueTask<IActionResult> ResendCodeAsync([FromBody] ResendCodeRequest request)
    {
        var result = await sender.Send(new ResendCodeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify code")]
    [EndpointDescription("Verify code")]
    [ProducesResponseType(200)]
    [HttpPost("code/verify")]
    public async ValueTask<IActionResult> VerifyCodeAsync([FromBody] VerifyCodeRequest request)
    {
        var result = await sender.Send(new VerifyCodeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify authenticator app code")]
    [EndpointDescription("Verify authenticator app code")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/verify")]
    public async ValueTask<IActionResult> VerifyAuthenticatorCodeAsync(
        [FromBody] VerifyAuthenticatorCodeRequest request)
    {
        var result = await sender.Send(new VerifyAuthenticatorCodeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify passkey verification")]
    [EndpointDescription("Verify passkey verification")]
    [ProducesResponseType(200)]
    [HttpPost("passkey/verify")]
    public async ValueTask<IActionResult> VerifyPasskeyAsync(
        [FromBody] VerifyPasskeyRequest request)
    {
        var result = await sender.Send(new VerifyPasskeyCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}