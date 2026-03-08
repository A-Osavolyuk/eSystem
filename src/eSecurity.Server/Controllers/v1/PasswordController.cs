using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Password.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class PasswordController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Add password")]
    [EndpointDescription("Add password")]
    [ProducesResponseType(200)]
    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> AddAsync([FromBody] AddPasswordRequest request)
    {
        var result = await _sender.Send(new AddPasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Change password")]
    [EndpointDescription("Change password")]
    [ProducesResponseType(200)]
    [HttpPost("change")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeAsync([FromBody] ChangePasswordRequest request)
    {
        var result = await _sender.Send(new ChangePasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Set password")]
    [EndpointDescription("Set password")]
    [ProducesResponseType(200)]
    [HttpPost("set")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> SetAsync([FromBody] SetPasswordRequest request)
    {
        var result = await _sender.Send(new SetPasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Reset password")]
    [EndpointDescription("Reset password")]
    [ProducesResponseType(200)]
    [HttpPost("reset")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ResetAsync([FromBody] ResetPasswordRequest request)
    {
        var result = await _sender.Send(new ResetPasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Forgot password request")]
    [EndpointDescription("Forgot password request")]
    [ProducesResponseType(200)]
    [HttpPost("forgot/request")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> RequestForgotAsync([FromBody] ForgotPasswordRequest request)
    {
        var result = await _sender.Send(new ForgotPasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Forgot password confirmation")]
    [EndpointDescription("Forgot password confirmation")]
    [ProducesResponseType(200)]
    [HttpPost("forgot/confirm")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ConfirmForgotAsync([FromBody] ConfirmForgotPasswordRequest request)
    {
        var result = await _sender.Send(new ConfirmForgotPasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
}