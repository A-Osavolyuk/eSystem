using eSecurity.Core.Common.Requests;
using eSecurity.Server.Common.Filters;
using eSecurity.Server.Features.Account.Commands;
using eSecurity.Server.Features.Account.Queries;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class AccountController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Load sign in session")]
    [EndpointDescription("Load sign in session")]
    [ProducesResponseType(200)]
    [HttpPost("sign-in/session/{sid:guid}")]
    public async ValueTask<IActionResult> LoadSignSessionAsync(Guid sid)
    {
        var result = await _sender.Send(new LoadSignInSessionCommand(sid));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Sign in")]
    [EndpointDescription("Sign in")]
    [ProducesResponseType(200)]
    [HttpPost("sign-in")]
    public async ValueTask<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        var result = await _sender.Send(new SignInCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Sign up")]
    [EndpointDescription("Sign up")]
    [ProducesResponseType(200)]
    [HttpPost("sign-up")]
    [RequireHeaders(HeaderTypes.XLocale, HeaderTypes.XTimezone)]
    public async ValueTask<IActionResult> SignUpAsync([FromBody] SignUpRequest request)
    {
        var result = await _sender.Send(new SignUpCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check")]
    [EndpointDescription("Check")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckAccountRequest request)
    {
        var result = await _sender.Send(new CheckAccountCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Recover")]
    [EndpointDescription("Recover")]
    [ProducesResponseType(200)]
    [HttpPost("recover")]
    public async ValueTask<IActionResult> RecoverAsync([FromBody] RecoverAccountRequest request)
    {
        var result = await _sender.Send(new RecoverAccountCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Unlock")]
    [EndpointDescription("Unlock")]
    [ProducesResponseType(200)]
    [HttpPost("unlock")]
    public async ValueTask<IActionResult> UnlockAsync([FromBody] UnlockAccountRequest request)
    {
        var result = await _sender.Send(new UnlockAccountCommand(request));
        return HttpContext.HandleResult(result);
    }
}