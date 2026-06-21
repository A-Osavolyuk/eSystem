using eSecurity.Core.Requests.Verification;
using eSecurity.Idp.Features.Verification.AuthenticatorApp;
using eSecurity.Idp.Features.Verification.EmailOtp;
using eSecurity.Idp.Features.Verification.SoftwareKey;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VerificationController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("Send email OTP")]
    [EndpointDescription("Send email OTP")]
    [ProducesResponseType(200)]
    [HttpPost("email-otp/send")]
    public async ValueTask<IActionResult> SendEmailOtpAsync([FromBody] SendEmailOtpRequest request)
    {
        var result = await _mediator.Send(new SendEmailOtpCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Resend email OTP")]
    [EndpointDescription("Resend email OTP")]
    [ProducesResponseType(200)]
    [HttpPost("email-otp/resend")]
    public async ValueTask<IActionResult> ResendEmailOtpAsync([FromBody] ResendEmailOtpRequest request)
    {
        var result = await _mediator.Send(new ResendEmailOtpCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verify email OTP")]
    [EndpointDescription("Verify email OTP")]
    [ProducesResponseType(200)]
    [HttpPost("email-otp/verify")]
    public async ValueTask<IActionResult> VerifyEmailOtpAsync([FromBody] VerifyEmailOtpRequest request)
    {
        var result = await _mediator.Send(new VerifyEmailOtpCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verify authenticator app")]
    [EndpointDescription("Verify authenticator app")]
    [ProducesResponseType(200)]
    [HttpPost("authenticator-app/verify")]
    public async ValueTask<IActionResult> VerifyAuthenticatorAppAsync([FromBody] VerifyAuthenticatorAppRequest request)
    {
        var result = await _mediator.Send(new VerifyAuthenticatorAppCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verify software key")]
    [EndpointDescription("Verify software key")]
    [ProducesResponseType(200)]
    [HttpPost("software-key/verify")]
    public async ValueTask<IActionResult> VerifySoftwareKeyAsync([FromBody] VerifySoftwareKeyRequest request)
    {
        var result = await _mediator.Send(new VerifySoftwareKeyCommand(request));
        return HttpContext.HandleResult(result);
    }
}