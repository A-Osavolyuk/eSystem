using eShop.Auth.Api.Features.Users.Commands;
using eShop.Auth.Api.Features.Users.Queries;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get user")]
    [EndpointDescription("Gets user")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpGet("{id:guid}")]
    public async ValueTask<ActionResult<Response>> GetUserAsync(Guid id)
    {
        var result = await sender.Send(new GetUserQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user roles")]
    [EndpointDescription("Gets user roles")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadRolesPolicy")]
    [HttpGet("{id:guid}/roles")]
    public async ValueTask<ActionResult<Response>> GetUserRolesAsync(Guid id)
    {
        var result = await sender.Send(new GetUserRolesQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get personal data")]
    [EndpointDescription("Get personal data")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}/personal-data")]
    [Authorize]
    public async ValueTask<ActionResult<Response>> GetPersonalDataAsync(Guid id)
    {
        var result = await sender.Send(new GetPersonalDataQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get lockout state")]
    [EndpointDescription("Get lockout state")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}/lockout-state")]
    [AllowAnonymous]
    public async ValueTask<ActionResult<Response>> GetStateAsync(Guid id)
    {
        var result = await sender.Send(new GetLockoutStateQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get two-factor state")]
    [EndpointDescription("Get two-factor state")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}/two-factor/state")]
    [AllowAnonymous]
    public async ValueTask<ActionResult<Response>> GetTwoFactorAuthenticationState(Guid id)
    {
        var result = await sender.Send(new GetTwoFactorStateQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithResult(s.Value!)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user two-factor providers")]
    [EndpointDescription("Get user two-factor providers")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}/two-factor/providers")]
    [AllowAnonymous]
    public async ValueTask<ActionResult<Response>> GetUserTwoFactorProvidersState(Guid id)
    {
        var result = await sender.Send(new GetUserProvidersQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithResult(s.Value!)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Create user account")]
    [EndpointDescription("Create a user account")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "CreateUserPolicy")]
    [HttpPost]
    public async ValueTask<ActionResult<Response>> CreateUserAccount([FromBody] CreateUserAccountRequest request)
    {
        var result = await sender.Send(new CreateUserAccountCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change username")]
    [EndpointDescription("Change username")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPatch("{id:guid}/username")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ChangeUsernameAsync([FromBody] ChangeUserNameRequest request)
    {
        var result = await sender.Send(new ChangeUserNameCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Add personal data")]
    [EndpointDescription("Add personal data")]
    [ProducesResponseType(200)]
    [HttpPost("{id:guid}/personal-data")]
    [Authorize]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> AddPersonalDataAsync([FromBody] AddPersonalDataRequest request)
    {
        var result = await sender.Send(new AddPersonalDataCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change personal data")]
    [EndpointDescription("Change personal data")]
    [ProducesResponseType(200)]
    [HttpPut("{id:guid}/personal-data")]
    [Authorize]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ChangePersonalDataAsync([FromBody] ChangePersonalDataRequest request)
    {
        var result = await sender.Send(new ChangePersonalDataCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Delete user account")]
    [EndpointDescription("Deletes user account")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "DeleteUsersPolicy")]
    [HttpDelete("{id:guid}")]
    public async ValueTask<ActionResult<Response>> DeleteUserAccountAsync(Guid id)
    {
        var result = await sender.Send(new DeleteUserAccountCommand(id));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}