using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Account.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class AccountController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Sign in")]
    [EndpointDescription("Sign in")]
    [ProducesResponseType(200)]
    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        var result = await _sender.Send(new SignInCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Sign up")]
    [EndpointDescription("Sign up")]
    [ProducesResponseType(200)]
    [HttpPost("sign-up")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> SignUpAsync([FromBody] SignUpRequest request)
    {
        var result = await _sender.Send(new SignUpCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Check")]
    [EndpointDescription("Check")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckAccountRequest request)
    {
        var result = await _sender.Send(new CheckAccountCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Recover")]
    [EndpointDescription("Recover")]
    [ProducesResponseType(200)]
    [HttpPost("recover")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> RecoverAsync([FromBody] RecoverAccountRequest request)
    {
        var result = await _sender.Send(new RecoverAccountCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Unlock")]
    [EndpointDescription("Unlock")]
    [ProducesResponseType(200)]
    [HttpPost("unlock")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> UnlockAsync([FromBody] UnlockAccountRequest request)
    {
        var result = await _sender.Send(new UnlockAccountCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}