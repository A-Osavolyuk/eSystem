using eSecurity.Server.Features.Users.Queries;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class UserController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Get user")]
    [EndpointDescription("Get user")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's 2FA methods")]
    [EndpointDescription("Get user's 2FA methods")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/2fa/methods")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserTwoFactorMethodsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserTwoFactorMethodsQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's verification methods")]
    [EndpointDescription("Get user's verification methods")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/verification/methods")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserVerificationMethodsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserVerificationDataQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's personal data")]
    [EndpointDescription("Get user's personal data")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/privacy")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserPersonalDataAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserPersonalQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's lockout state")]
    [EndpointDescription("Get user's lockout state")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/lockout")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserLogoutStateAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserLockoutQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's linked accounts")]
    [EndpointDescription("Get user's linked accounts")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/linked-accounts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserLinkedAccountsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserLinkedAccountDataQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's login methods")]
    [EndpointDescription("Get user's login methods")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/login-methods")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserLoginMethodsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserLoginMethodsQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's devices")]
    [EndpointDescription("Get user's devices")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/devices")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserDevicesAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserDevicesQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's device")]
    [EndpointDescription("Get user's device")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/devices/{deviceId:guid}")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserDevicesAsync(Guid userId, Guid deviceId)
    {
        var result = await _sender.Send(new GetUserDeviceQuery(userId, deviceId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's emails")]
    [EndpointDescription("Get user's emails")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/emails")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserEmailsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserEmailsQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's primary email")]
    [EndpointDescription("Get user's primary email")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/emails/primary")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserPrimaryEmailAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserPrimaryEmailQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's phone numbers")]
    [EndpointDescription("Get user's phone numbers")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/phone-numbers")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserPhoneNumbersAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserPhoneNumbersQuery(userId));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Get user's primary phone number")]
    [EndpointDescription("Get user's primary phone number")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/phone-numbers/primary")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserPrimaryPhoneNumberAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserPrimaryPhoneNumberQuery(userId));
        return HttpContext.HandleResult(result);
    }
}