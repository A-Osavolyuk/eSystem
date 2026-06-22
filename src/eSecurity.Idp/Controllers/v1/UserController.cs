using eSecurity.Idp.Features.Users;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("Get user's 2FA methods")]
    [EndpointDescription("Get user's 2FA methods")]
    [ProducesResponseType(200)]
    [HttpGet("2fa/methods")]
    public async ValueTask<IActionResult> GetUserTwoFactorMethodsAsync()
    {
        var result = await _mediator.Send(new GetUserTwoFactorMethodsQuery());
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's verification methods")]
    [EndpointDescription("Get user's verification methods")]
    [ProducesResponseType(200)]
    [HttpGet("verification/methods")]
    public async ValueTask<IActionResult> GetUserVerificationMethodsAsync()
    {
        var result = await _mediator.Send(new GetUserVerificationDataQuery());
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's linked accounts")]
    [EndpointDescription("Get user's linked accounts")]
    [ProducesResponseType(200)]
    [HttpGet("linked-accounts")]
    public async ValueTask<IActionResult> GetUserLinkedAccountsAsync()
    {
        var result = await _mediator.Send(new GetUserLinkedAccountDataQuery());
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's login methods")]
    [EndpointDescription("Get user's login methods")]
    [ProducesResponseType(200)]
    [HttpGet("login-methods")]
    public async ValueTask<IActionResult> GetUserLoginMethodsAsync()
    {
        var result = await _mediator.Send(new GetUserLoginMethodsQuery());
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's devices")]
    [EndpointDescription("Get user's devices")]
    [ProducesResponseType(200)]
    [HttpGet("devices")]
    public async ValueTask<IActionResult> GetUserDevicesAsync()
    {
        var result = await _mediator.Send(new GetUserDevicesQuery());
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's emails")]
    [EndpointDescription("Get user's emails")]
    [ProducesResponseType(200)]
    [HttpGet("emails")]
    public async ValueTask<IActionResult> GetUserEmailsAsync()
    {
        var result = await _mediator.Send(new GetUserEmailsQuery());
        return HttpContext.HandleResult(result);
    }
}