using eSecurity.Idp.Features.Password;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class PasswordController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("Add password")]
    [EndpointDescription("Add password")]
    [ProducesResponseType(200)]
    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> AddAsync([FromBody] AddPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Change password")]
    [EndpointDescription("Change password")]
    [ProducesResponseType(200)]
    [HttpPost("change")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeAsync([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Set password")]
    [EndpointDescription("Set password")]
    [ProducesResponseType(200)]
    [HttpPost("set")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SetAsync([FromBody] SetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Reset password")]
    [EndpointDescription("Reset password")]
    [ProducesResponseType(200)]
    [HttpPost("reset")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ResetAsync([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Forgot password request")]
    [EndpointDescription("Forgot password request")]
    [ProducesResponseType(200)]
    [HttpPost("forgot/request")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> RequestForgotAsync([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Forgot password confirmation")]
    [EndpointDescription("Forgot password confirmation")]
    [ProducesResponseType(200)]
    [HttpPost("forgot/confirm")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ConfirmForgotAsync([FromBody] ConfirmForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
}