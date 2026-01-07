using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Account.Commands;
using eSecurity.Server.Features.Account.Queries;
using eSystem.Core.Common.Http.Constants;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class AccountController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Get sign in session")]
    [EndpointDescription("Get sign in session")]
    [ProducesResponseType(200)]
    [HttpGet("sign-in/session/{sid:guid}")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetSignSessionAsync(Guid sid)
    {
        var result = await _sender.Send(new GetSignInSessionQuery(sid));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Sign in")]
    [EndpointDescription("Sign in")]
    [ProducesResponseType(200)]
    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        var result = await _sender.Send(new SignInCommand(request));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Sign up")]
    [EndpointDescription("Sign up")]
    [ProducesResponseType(200)]
    [HttpPost("sign-up")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> SignUpAsync([FromBody] SignUpRequest request)
    {
        var result = await _sender.Send(new SignUpCommand(request));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Check")]
    [EndpointDescription("Check")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckAccountRequest request)
    {
        var result = await _sender.Send(new CheckAccountCommand(request));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Recover")]
    [EndpointDescription("Recover")]
    [ProducesResponseType(200)]
    [HttpPost("recover")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> RecoverAsync([FromBody] RecoverAccountRequest request)
    {
        var result = await _sender.Send(new RecoverAccountCommand(request));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Unlock")]
    [EndpointDescription("Unlock")]
    [ProducesResponseType(200)]
    [HttpPost("unlock")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> UnlockAsync([FromBody] UnlockAccountRequest request)
    {
        var result = await _sender.Send(new UnlockAccountCommand(request));
        return ResultHandler.Handle(result);
    }
}