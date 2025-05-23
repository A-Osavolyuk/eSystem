using eShop.Auth.Api.Features.Permissions.Queries;
using eShop.Auth.Api.Features.Roles.Commands;
using eShop.Auth.Api.Features.Roles.Queries;
using eShop.Auth.Api.Features.Users.Commands;
using eShop.Auth.Api.Features.Users.Queries;
using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class AdminController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Find user by email")]
    [EndpointDescription("Finds user by email")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadUsersPolicy")]
    [HttpGet("find-user-by-email/{email}")]
    public async ValueTask<ActionResult<Response>> FindUserByEmailAsync(string email)
    {
        var result = await sender.Send(new FindUserByEmailQuery(email));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Find user by id")]
    [EndpointDescription("Finds user by id")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadUsersPolicy")]
    [HttpGet("find-user-by-id/{id:guid}")]
    public async ValueTask<ActionResult<Response>> FindUserByIdAsync(Guid id)
    {
        var result = await sender.Send(new FindUserByIdQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get all users")]
    [EndpointDescription("Gets all users")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadUsersPolicy")]
    [HttpGet("get-all-users")]
    public async ValueTask<ActionResult<Response>> GetAllUsersAsync()
    {
        var result = await sender.Send(new GetUsersListQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get roles")]
    [EndpointDescription("Gets all roles")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadRolesPolicy")]
    [HttpGet("get-roles")]
    public async ValueTask<ActionResult<Response>> GetRolesListAsync()
    {
        var result = await sender.Send(new GetRolesListQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get user roles")]
    [EndpointDescription("Gets user roles")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadRolesPolicy")]
    [HttpGet("get-user-roles/{id:guid}")]
    public async ValueTask<ActionResult<Response>> GetUserRolesAsync(Guid id)
    {
        var result = await sender.Send(new GetUserRolesQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get permissions")]
    [EndpointDescription("Gets all permissions")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadPermissionsPolicy")]
    [HttpGet("get-permissions")]
    public async ValueTask<ActionResult<Response>> GetPermissionsListAsync()
    {
        var result = await sender.Send(new GetPermissionsListQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Assign role")]
    [EndpointDescription("Assigns role to user")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "AssignRolesPolicy")]
    [HttpPost("assign-role")]
    public async ValueTask<ActionResult<Response>> AssignRoleAsync([FromBody] AssignRoleRequest request)
    {
        var result = await sender.Send(new AssignRoleCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Grant permissions")]
    [EndpointDescription("Grant permission")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "GrantPermissionsPolicy")]
    [HttpPost("grant-permissions")]
    public async ValueTask<ActionResult<Response>> GrantPermissionsAsync([FromBody] GrantPermissionRequest request)
    {
        var result = await sender.Send(new GrantPermissionCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Create role")]
    [EndpointDescription("Creates a role")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "CreateRolesPolicy")]
    [HttpPost("create-role")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> CreateRoleAsync([FromBody] CreateRoleRequest request)
    {
        var result = await sender.Send(new CreateRoleCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Create user account")]
    [EndpointDescription("Create a user account")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "CreateUserPolicy")]
    [HttpPost("create-user-account")]
    public async ValueTask<ActionResult<Response>> CreateUserAccount([FromBody] CreateUserAccountRequest request)
    {
        var result = await sender.Send(new CreateUserAccountCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Unassign roles")]
    [EndpointDescription("Unassign roles")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UnassignRolesPolicy")]
    [HttpDelete("unassign-roles")]
    public async ValueTask<ActionResult<Response>> UnassignRolesAsync([FromBody] UnassignRolesRequest request)
    {
        var result = await sender.Send(new UnassignRolesCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Delete role")]
    [EndpointDescription("Deletes role")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "DeleteRolesPolicy")]
    [HttpDelete("delete-role/{id:guid}")]
    public async ValueTask<ActionResult<Response>> DeleteRoleAsync(Guid id)
    {
        var result = await sender.Send(new DeleteRoleCommand(id));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Revoke permission")]
    [EndpointDescription("Revoke permission")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "RevokePermissionPolicy")]
    [HttpDelete("revoke-permission")]
    public async ValueTask<ActionResult<Response>> RevokePermissionAsync(
        [FromBody] RevokePermissionRequest request)
    {
        var result = await sender.Send(new RevokePermissionCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Delete user account")]
    [EndpointDescription("Deletes user account")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "DeleteUsersPolicy")]
    [HttpDelete("delete-user-account")]
    public async ValueTask<ActionResult<Response>> DeleteUserAccountAsync([FromBody] DeleteUserAccountRequest request)
    {
        var result = await sender.Send(new DeleteUserAccountCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}