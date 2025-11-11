using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Password.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class PasswordController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Add password")]
    [EndpointDescription("Add password")]
    [ProducesResponseType(200)]
    [HttpPost("add")]
    [Authorize]
    public async ValueTask<IActionResult> AddAsync([FromBody] AddPasswordRequest request)
    {
        var result = await sender.Send(new AddPasswordCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change password")]
    [EndpointDescription("Change password")]
    [ProducesResponseType(200)]
    [HttpPost("change")]
    [Authorize]
    public async ValueTask<IActionResult> ChangeAsync([FromBody] ChangePasswordRequest request)
    {
        var result = await sender.Send(new ChangePasswordCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Reset password")]
    [EndpointDescription("Reset password")]
    [ProducesResponseType(200)]
    [HttpPost("reset")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ResetAsync([FromBody] ResetPasswordRequest request)
    {
        var result = await sender.Send(new ResetPasswordCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Remove password")]
    [EndpointDescription("Remove password")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemovePasswordRequest request)
    {
        var result = await sender.Send(new RemovePasswordCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Check password")]
    [EndpointDescription("Check password")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckPasswordRequest request)
    {
        var result = await sender.Send(new CheckPasswordCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Forgot password")]
    [EndpointDescription("Forgot password")]
    [ProducesResponseType(200)]
    [HttpPost("forgot")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> ForgotAsync([FromBody] ForgotPasswordRequest request)
    {
        var result = await sender.Send(new ForgotPasswordCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}