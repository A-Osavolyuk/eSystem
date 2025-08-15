using eShop.Auth.Api.Features.Users.Commands;
using eShop.Auth.Api.Features.Users.Queries;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get user")]
    [EndpointDescription("Gets user")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}")]
    public async ValueTask<IActionResult> GetUserAsync(Guid id)
    {
        var result = await sender.Send(new GetUserQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user personal data")]
    [EndpointDescription("Gets user personal data")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}/personal")]
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
    [HttpGet("{id:guid}/security")]
    public async ValueTask<IActionResult> GetUserSecurityDataAsync(Guid id)
    {
        var result = await sender.Send(new GetUserSecurityQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user roles")]
    [EndpointDescription("Gets user roles")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}/roles")]
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
    [HttpGet("{id:guid}/lockout")]
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
    [HttpGet("{id:guid}/2fa/providers")]
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
    [HttpPatch("{id:guid}/username")]
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
    [HttpPost("{id:guid}/personal")]
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
    [HttpPut("{id:guid}/personal")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ChangePersonalDataAsync([FromBody] ChangePersonalDataRequest request)
    {
        var result = await sender.Send(new ChangePersonalDataCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
}