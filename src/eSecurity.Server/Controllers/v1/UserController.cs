using eSecurity.Server.Features.Users.Queries;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]/{subject}")]
public class UserController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Get user's 2FA methods")]
    [EndpointDescription("Get user's 2FA methods")]
    [ProducesResponseType(200)]
    [HttpGet("2fa/methods")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserTwoFactorMethodsAsync(string subject)
    {
        var result = await _sender.Send(new GetUserTwoFactorMethodsQuery(subject));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's verification methods")]
    [EndpointDescription("Get user's verification methods")]
    [ProducesResponseType(200)]
    [HttpGet("verification/methods")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserVerificationMethodsAsync(string subject)
    {
        var result = await _sender.Send(new GetUserVerificationDataQuery(subject));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's linked accounts")]
    [EndpointDescription("Get user's linked accounts")]
    [ProducesResponseType(200)]
    [HttpGet("linked-accounts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserLinkedAccountsAsync(string subject)
    {
        var result = await _sender.Send(new GetUserLinkedAccountDataQuery(subject));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's login methods")]
    [EndpointDescription("Get user's login methods")]
    [ProducesResponseType(200)]
    [HttpGet("login-methods")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserLoginMethodsAsync(string subject)
    {
        var result = await _sender.Send(new GetUserLoginMethodsQuery(subject));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's devices")]
    [EndpointDescription("Get user's devices")]
    [ProducesResponseType(200)]
    [HttpGet("devices")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserDevicesAsync(string subject)
    {
        var result = await _sender.Send(new GetUserDevicesQuery(subject));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's emails")]
    [EndpointDescription("Get user's emails")]
    [ProducesResponseType(200)]
    [HttpGet("emails")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserEmailsAsync(string subject)
    {
        var result = await _sender.Send(new GetUserEmailsQuery(subject));
        return HttpContext.HandleResult(result);
    }
}