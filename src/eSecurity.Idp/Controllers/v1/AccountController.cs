using eSecurity.Idp.Common.Filters;
using eSecurity.Idp.Features.Account.Commands;
using eSecurity.Idp.Features.Account.Queries;
using eSecurity.Core.Requests;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class AccountController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [EndpointSummary("Get authentication session")]
    [EndpointDescription("Get authentication session")]
    [ProducesResponseType(200)]
    [HttpGet("session/{sid:guid}")]
    public async ValueTask<IActionResult> GetAuthenticationSessionAsync(Guid sid)
    {
        var result = await _mediator.Send(new GetAuthenticationSessionQuery(sid));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Sign in")]
    [EndpointDescription("Sign in")]
    [ProducesResponseType(200)]
    [HttpPost("sign-in")]
    public async ValueTask<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        var result = await _mediator.Send(new SignInCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Sign up")]
    [EndpointDescription("Sign up")]
    [ProducesResponseType(200)]
    [HttpPost("sign-up")]
    [RequireHeaders(HeaderTypes.XLocale, HeaderTypes.XTimezone)]
    public async ValueTask<IActionResult> SignUpAsync([FromBody] SignUpRequest request)
    {
        var result = await _mediator.Send(new SignUpCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Complete sign up")]
    [EndpointDescription("Complete sign up")]
    [ProducesResponseType(200)]
    [HttpPost("sign-up/complete")]
    public async ValueTask<IActionResult> CompleteSignUpAsync([FromBody] CompleteSignUpRequest request)
    {
        var result = await _mediator.Send(new CompleteSignUpCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check")]
    [EndpointDescription("Check")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckAccountRequest request)
    {
        var result = await _mediator.Send(new CheckAccountCommand(request));
        return HttpContext.HandleResult(result);
    }
}