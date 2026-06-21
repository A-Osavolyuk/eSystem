using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Requests.Email.Reset;
using eSecurity.Core.Requests.Email.Verification;
using eSecurity.Idp.Features.Email;
using eSecurity.Idp.Features.Email.Change;
using eSecurity.Idp.Features.Email.Reset;
using eSecurity.Idp.Features.Email.Verification;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class EmailController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("Confirm email verification")]
    [EndpointDescription("Confirm email verification")]
    [ProducesResponseType(200)]
    [HttpPost("verification/confirm")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> VerifyEmailAsync([FromBody] VerifyEmailRequest request)
    {
        var result = await _mediator.Send(new VerifyEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Email verification")]
    [EndpointDescription("Email verification")]
    [ProducesResponseType(200)]
    [HttpPost("verification/sent-otp")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SendEmailVerificationOtpAsync([FromBody] SendEmailVerificationOtpRequest otpRequest)
    {
        var result = await _mediator.Send(new SendEmailVerificationOtpCommand(otpRequest));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Send email change")]
    [EndpointDescription("Send email change")]
    [ProducesResponseType(200)]
    [HttpPost("change/send-otp")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SendEmailChangeOtpAsync([FromBody] SendEmailChangeOtpRequest otpRequest)
    {
        var result = await _mediator.Send(new SendEmailChangeOtpCommand(otpRequest));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Confirm email change")]
    [EndpointDescription("Confirm email change")]
    [ProducesResponseType(200)]
    [HttpPost("change/confirm")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailRequest request)
    {
        var result = await _mediator.Send(new ChangeEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Send email reset")]
    [EndpointDescription("Send email reset")]
    [ProducesResponseType(200)]
    [HttpPost("reset/send-otp")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SendEmailResetOtpAsync([FromBody] SendEmailResetOtpRequest otpRequest)
    {
        var result = await _mediator.Send(new SendEmailResetOtpCommand(otpRequest));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Confirm email reset")]
    [EndpointDescription("Confirm email reset")]
    [ProducesResponseType(200)]
    [HttpPost("reset/confirm")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ResetEmailAsync([FromBody] ResetEmailRequest request)
    {
        var result = await _mediator.Send(new ResetEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check email")]
    [EndpointDescription("Check email")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckEmailRequest request)
    {
        var result = await _mediator.Send(new CheckEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Add email")]
    [EndpointDescription("Add email")]
    [ProducesResponseType(200)]
    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> AddAsync([FromBody] AddEmailRequest request)
    {
        var result = await _mediator.Send(new AddEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Remove email")]
    [EndpointDescription("Remove email")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemoveEmailRequest request)
    {
        var result = await _mediator.Send(new RemoveEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
}