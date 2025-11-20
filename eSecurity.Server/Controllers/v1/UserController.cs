using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Users.Commands;
using eSecurity.Server.Features.Users.Queries;
using eSystem.Core.Common.Http.Constants;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class UserController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Change username")]
    [EndpointDescription("Change username")]
    [ProducesResponseType(200)]
    [HttpPost("username/change")]
    [Authorize]
    public async ValueTask<IActionResult> EnableAsync([FromBody] ChangeUsernameRequest request)
    {
        var result = await _sender.Send(new ChangeUsernameCommand(request));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user")]
    [EndpointDescription("Get user")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's state")]
    [EndpointDescription("Get user's state")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/state")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserStateAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserStateQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's roles")]
    [EndpointDescription("Get user's roles")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/roles")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserRolesAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserRolesQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's 2FA methods")]
    [EndpointDescription("Get user's 2FA methods")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/2fa/methods")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserTwoFactorMethodsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserTwoFactorMethodsQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's verification methods")]
    [EndpointDescription("Get user's verification methods")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/verification/methods")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserVerificationMethodsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserVerificationDataQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's personal data")]
    [EndpointDescription("Get user's personal data")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/privacy")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserPersonalDataAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserPersonalQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's lockout state")]
    [EndpointDescription("Get user's lockout state")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/lockout")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserLogoutStateAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserLockoutQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's linked accounts")]
    [EndpointDescription("Get user's linked accounts")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/linked-accounts")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserLinkedAccountsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserLinkedAccountDataQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's login methods")]
    [EndpointDescription("Get user's login methods")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/login-methods")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserLoginMethodsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserLoginMethodsQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's devices")]
    [EndpointDescription("Get user's devices")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/devices")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserDevicesAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserDevicesQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's device")]
    [EndpointDescription("Get user's device")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/devices/{deviceId:guid}")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserDevicesAsync(Guid userId, Guid deviceId)
    {
        var result = await _sender.Send(new GetUserDeviceQuery(userId, deviceId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's emails")]
    [EndpointDescription("Get user's emails")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/emails")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserEmailsAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserEmailsQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's primary email")]
    [EndpointDescription("Get user's primary email")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/emails/primary")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserPrimaryEmailAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserPrimaryEmailQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's phone numbers")]
    [EndpointDescription("Get user's phone numbers")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/phone-numbers")]
    [Authorize]
    public async ValueTask<IActionResult> GetUserPhoneNumbersAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserPhoneNumbersQuery(userId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Get user's primary phone number")]
    [EndpointDescription("Get user's primary phone number")]
    [ProducesResponseType(200)]
    [HttpGet("{userId:guid}/phone-numbers/primary")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserPrimaryPhoneNumberAsync(Guid userId)
    {
        var result = await _sender.Send(new GetUserPrimaryPhoneNumberQuery(userId));
        return ResultHandler.Handle(result);
    }
}