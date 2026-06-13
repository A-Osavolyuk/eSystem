using eSecurity.Idp.Features.Email.Commands;
using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Requests.Email.Verification;
using eSecurity.Idp.Features.Email.Change;
using eSecurity.Idp.Features.Email.Verification;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class EmailController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Confirm email verification")]
    [EndpointDescription("Confirm email verification")]
    [ProducesResponseType(200)]
    [HttpPost("verification/confirm")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ConfirmVerificationAsync([FromBody] ConfirmEmailVerificationRequest request)
    {
        var result = await _sender.Send(new ConfirmEmailVerificationCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Send email verification")]
    [EndpointDescription("Send email verification")]
    [ProducesResponseType(200)]
    [HttpPost("verification/send")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SendVerificationAsync([FromBody] SendEmailVerificationRequest request)
    {
        var result = await _sender.Send(new SendEmailVerificationCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Resend email verification")]
    [EndpointDescription("Resend email verification")]
    [ProducesResponseType(200)]
    [HttpPost("verification/resend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ResendVerificationAsync([FromBody] ResendEmailVerificationRequest request)
    {
        var result = await _sender.Send(new ResendEmailVerificationCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Send email change")]
    [EndpointDescription("Send email change")]
    [ProducesResponseType(200)]
    [HttpPost("change/send")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SendEmailAsyncAsync([FromBody] SendEmailChangeRequest request)
    {
        var result = await _sender.Send(new SendEmailChangeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Resend email change")]
    [EndpointDescription("Resend email change")]
    [ProducesResponseType(200)]
    [HttpPost("change/resend")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ResendEmailAsyncAsync([FromBody] ResendEmailChangeRequest request)
    {
        var result = await _sender.Send(new ResendEmailChangeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Can change email")]
    [EndpointDescription("Can change email")]
    [ProducesResponseType(200)]
    [HttpPost("change/can-change")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> CanChangeEmailAsync([FromBody] CanChangeEmailRequest request)
    {
        var result = await _sender.Send(new CanChangeEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Confirm email change")]
    [EndpointDescription("Confirm email change")]
    [ProducesResponseType(200)]
    [HttpPost("change/confirm")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ConfirmEmailChangeAsync([FromBody] ConfirmEmailChangeRequest request)
    {
        var result = await _sender.Send(new ConfirmEmailChangeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check email")]
    [EndpointDescription("Check email")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckEmailRequest request)
    {
        var result = await _sender.Send(new CheckEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Add email")]
    [EndpointDescription("Add email")]
    [ProducesResponseType(200)]
    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> AddAsync([FromBody] AddEmailRequest request)
    {
        var result = await _sender.Send(new AddEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Reset email")]
    [EndpointDescription("Reset email")]
    [ProducesResponseType(200)]
    [HttpPost("reset")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ResetAsync([FromBody] ResetEmailRequest request)
    {
        var result = await _sender.Send(new ResetEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Remove email")]
    [EndpointDescription("Remove email")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemoveEmailRequest request)
    {
        var result = await _sender.Send(new RemoveEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
}