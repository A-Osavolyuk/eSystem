using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Password.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;

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
    
    [EndpointSummary("Remove password")]
    [EndpointDescription("Remove password")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemovePasswordRequest request)
    {
        var result = await _sender.Send(new RemovePasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check password")]
    [EndpointDescription("Check password")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckPasswordRequest request)
    {
        var result = await _sender.Send(new CheckPasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Forgot password")]
    [EndpointDescription("Forgot password")]
    [ProducesResponseType(200)]
    [HttpPost("forgot")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ForgotAsync([FromBody] ForgotPasswordRequest request)
    {
        var result = await _sender.Send(new ForgotPasswordCommand(request));
        return HttpContext.HandleResult(result);
    }
}