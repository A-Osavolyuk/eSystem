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

    [EndpointSummary("Get roles")]
    [EndpointDescription("Gets all roles")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadRolesPolicy")]
    [HttpGet("roles")]
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
    [HttpGet("user/{id:guid}/roles")]
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
    [HttpGet("permissions")]
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
}