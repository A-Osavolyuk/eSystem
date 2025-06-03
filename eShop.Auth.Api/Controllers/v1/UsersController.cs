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
    
    [EndpointSummary("Get user roles")]
    [EndpointDescription("Gets user roles")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadRolesPolicy")]
    [HttpGet("{id:guid}/roles")]
    public async ValueTask<ActionResult<Response>> GetUserRolesAsync(Guid id)
    {
        var result = await sender.Send(new GetRolesQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get lockout state")]
    [EndpointDescription("Get lockout state")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}/lockout-state")]
    [Authorize(Policy = "ReadUsersPolicy")]
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
    [HttpGet("{id:guid}/two-factor-state")]
    [Authorize]
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
    [HttpGet("{id:guid}/two-factor-providers")]
    [AllowAnonymous]
    public async ValueTask<ActionResult<Response>> GetUserTwoFactorProvidersState(Guid id)
    {
        var result = await sender.Send(new GetProvidersQuery(id));

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