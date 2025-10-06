using eShop.Auth.Api.Features.Verification.Commands;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Controllers.v1;

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
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
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
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
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
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify authenticator app code")]
    [EndpointDescription("Verify authenticator app code")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator/verify")]
    public async ValueTask<IActionResult> VerifyAuthenticatorCodeAsync([FromBody] VerifyAuthenticatorCodeRequest request)
    {
        var result = await sender.Send(new VerifyAuthenticatorCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}