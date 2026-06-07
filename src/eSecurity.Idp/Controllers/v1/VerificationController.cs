using eSecurity.Idp.Features.Verification.Commands;
using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Verification;
using eSecurity.Idp.Features.Verification.AuthenticatorApp;
using eSecurity.Idp.Features.Verification.EmailOtp;
using eSecurity.Idp.Features.Verification.SoftwareKey;
using eSecurity.Idp.Security.Authorization.Constants;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using VerificationRequest = eSecurity.Core.Requests.VerificationRequest;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VerificationController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Send code")]
    [EndpointDescription("Send code")]
    [ProducesResponseType(200)]
    [HttpPost("code/send")]
    public async ValueTask<IActionResult> SendCodeAsync([FromBody] SendCodeRequest request)
    {
        var result = await _sender.Send(new SendCodeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Resend code")]
    [EndpointDescription("Resend code")]
    [ProducesResponseType(200)]
    [HttpPost("code/resend")]
    public async ValueTask<IActionResult> ResendCodeAsync([FromBody] ResendCodeRequest request)
    {
        var result = await _sender.Send(new ResendCodeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verification request")]
    [EndpointDescription("Verification request")]
    [ProducesResponseType(200)]
    [HttpPost("request-verification")]
    public async ValueTask<IActionResult> VerificationRequestAsync([FromBody] VerificationRequest request)
    {
        var result = await _sender.Send(new VerificationCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Send email OTP")]
    [EndpointDescription("Send email OTP")]
    [ProducesResponseType(200)]
    [HttpPost("email-otp/send")]
    public async ValueTask<IActionResult> SendEmailOtpAsync()
    {
        var result = await _sender.Send(new SendEmailOtpCommand());
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Resend email OTP")]
    [EndpointDescription("Resend email OTP")]
    [ProducesResponseType(200)]
    [HttpPost("email-otp/resend")]
    public async ValueTask<IActionResult> ResendEmailOtpAsync()
    {
        var result = await _sender.Send(new ResendEmailOtpCommand());
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verify email OTP")]
    [EndpointDescription("Verify email OTP")]
    [ProducesResponseType(200)]
    [HttpPost("email-otp/verify")]
    public async ValueTask<IActionResult> VerifyEmailOtpAsync([FromBody] VerifyEmailOtpRequest request)
    {
        var result = await _sender.Send(new VerifyEmailOtpCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verify authenticator app")]
    [EndpointDescription("Verify authenticator app")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator-app/verify")]
    public async ValueTask<IActionResult> VerifyAuthenticatorAppAsync([FromBody] VerifyAuthenticatorAppRequest request)
    {
        var result = await _sender.Send(new VerifyAuthenticatorAppCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verify software key")]
    [EndpointDescription("Verify software key")]
    [ProducesResponseType(200)]
    [HttpPost("software-key/verify")]
    public async ValueTask<IActionResult> VerifySoftwareKeyAsync([FromBody] VerifySoftwareKeyRequest request)
    {
        var result = await _sender.Send(new VerifySoftwareKeyCommand(request));
        return HttpContext.HandleResult(result);
    }
}