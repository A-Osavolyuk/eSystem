using eSecurity.Idp.Features.Email;
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
    public async ValueTask<IActionResult> VerifyEmailAsync([FromBody] VerifyEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Email verification")]
    [EndpointDescription("Email verification")]
    [ProducesResponseType(200)]
    [HttpPost("verification/sent-otp")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SendEmailVerificationOtpAsync([FromBody] SendEmailVerificationOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Send email change")]
    [EndpointDescription("Send email change")]
    [ProducesResponseType(200)]
    [HttpPost("change/send-otp")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SendEmailChangeOtpAsync([FromBody] SendEmailChangeOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Confirm email change")]
    [EndpointDescription("Confirm email change")]
    [ProducesResponseType(200)]
    [HttpPost("change/confirm")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Send email reset")]
    [EndpointDescription("Send email reset")]
    [ProducesResponseType(200)]
    [HttpPost("reset/send-otp")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SendEmailResetOtpAsync([FromBody] SendEmailResetOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Confirm email reset")]
    [EndpointDescription("Confirm email reset")]
    [ProducesResponseType(200)]
    [HttpPost("reset/confirm")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ResetEmailAsync([FromBody] ResetEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check email")]
    [EndpointDescription("Check email")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Add email")]
    [EndpointDescription("Add email")]
    [ProducesResponseType(200)]
    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> AddAsync([FromBody] AddEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Remove email")]
    [EndpointDescription("Remove email")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemoveEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
}