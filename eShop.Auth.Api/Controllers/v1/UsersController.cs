using eShop.Auth.Api.Features.Users.Commands;
using eShop.Auth.Api.Features.Users.Queries;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]/{id:guid}")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get user")]
    [EndpointDescription("Gets user")]
    [ProducesResponseType(200)]
    [HttpGet]
    public async ValueTask<IActionResult> GetUserAsync(Guid id)
    {
        var result = await sender.Send(new GetUserQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user state")]
    [EndpointDescription("Gets user state")]
    [ProducesResponseType(200)]
    [HttpGet("state")]
    public async ValueTask<IActionResult> GetUserStateAsync(Guid id)
    {
        var result = await sender.Send(new GetUserStateQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user emails")]
    [EndpointDescription("Gets user emails")]
    [ProducesResponseType(200)]
    [HttpGet("emails")]
    public async ValueTask<IActionResult> GetUserEmailsAsync(Guid id)
    {
        var result = await sender.Send(new GetUserEmailsQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user primary email")]
    [EndpointDescription("Gets user primary email")]
    [ProducesResponseType(200)]
    [HttpGet("primary-email")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserPrimaryEmailAsync(Guid id)
    {
        var result = await sender.Send(new GetUserPrimaryEmailQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
        [EndpointSummary("Get user primary phone number")]
        [EndpointDescription("Gets user primary phone number")]
        [ProducesResponseType(200)]
        [HttpGet("primary-phone-number")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> GetUserPrimaryPhoneNumberAsync(Guid id)
        {
            var result = await sender.Send(new GetUserPrimaryPhoneNumberQuery(id));
    
            return result.Match(
                s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
                ErrorHandler.Handle);
        }
    
    [EndpointSummary("Get user phone numbers")]
    [EndpointDescription("Gets user phone numbers")]
    [ProducesResponseType(200)]
    [HttpGet("phone-numbers")]
    public async ValueTask<IActionResult> GetUserPhoneNumbersAsync(Guid id)
    {
        var result = await sender.Send(new GetUserPhoneNumbersQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user personal data")]
    [EndpointDescription("Gets user personal data")]
    [ProducesResponseType(200)]
    [HttpGet("personal")]
    public async ValueTask<IActionResult> GetUserPersonalDataAsync(Guid id)
    {
        var result = await sender.Send(new GetUserPersonalQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user security data")]
    [EndpointDescription("Gets user security data")]
    [ProducesResponseType(200)]
    [HttpGet("security")]
    public async ValueTask<IActionResult> GetUserSecurityDataAsync(Guid id)
    {
        var result = await sender.Send(new GetUserSecurityQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user devices")]
    [EndpointDescription("Gets user devices")]
    [ProducesResponseType(200)]
    [HttpGet("devices")]
    public async ValueTask<IActionResult> GetUserDevicesAsync(Guid id)
    {
        var result = await sender.Send(new GetUserDevicesQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user roles")]
    [EndpointDescription("Gets user roles")]
    [ProducesResponseType(200)]
    [HttpGet("roles")]
    public async ValueTask<IActionResult> GetUserRolesAsync(Guid id)
    {
        var result = await sender.Send(new GetUserRolesQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get lockout state")]
    [EndpointDescription("Get lockout state")]
    [ProducesResponseType(200)]
    [HttpGet("lockout")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetStateAsync(Guid id)
    {
        var result = await sender.Send(new GetUserLockoutQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user two-factor providers")]
    [EndpointDescription("Get user two-factor providers")]
    [ProducesResponseType(200)]
    [HttpGet("2fa/providers")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetUserTwoFactorProvidersState(Guid id)
    {
        var result = await sender.Send(new GetUserProvidersQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithResult(s.Value!)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change username")]
    [EndpointDescription("Change username")]
    [ProducesResponseType(200)]
    [HttpPatch("username")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ChangeUsernameAsync([FromBody] ChangeUsernameRequest request)
    {
        var result = await sender.Send(new ChangeUsernameCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Add personal data")]
    [EndpointDescription("Add personal data")]
    [ProducesResponseType(200)]
    [HttpPost("personal")]
    [ValidationFilter]
    public async ValueTask<IActionResult> AddPersonalDataAsync([FromBody] AddPersonalDataRequest request)
    {
        var result = await sender.Send(new AddPersonalDataCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change personal data")]
    [EndpointDescription("Change personal data")]
    [ProducesResponseType(200)]
    [HttpPut("personal")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ChangePersonalDataAsync([FromBody] ChangePersonalDataRequest request)
    {
        var result = await sender.Send(new ChangePersonalDataCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Remove personal data")]
    [EndpointDescription("Remove personal data")]
    [ProducesResponseType(200)]
    [HttpDelete("personal")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RemovePersonalDataAsync([FromBody] RemovePersonalDataRequest request)
    {
        var result = await sender.Send(new RemovePersonalDataCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
}