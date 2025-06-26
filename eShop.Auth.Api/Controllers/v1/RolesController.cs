using eShop.Auth.Api.Features.Roles.Commands;
using eShop.Auth.Api.Features.Roles.Queries;
using eShop.Auth.Api.Features.Users.Commands;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class RolesController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get roles")]
    [EndpointDescription("Gets all roles")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ReadRolesPolicy")]
    [HttpGet]
    public async ValueTask<ActionResult<Response>> GetRolesAsync()
    {
        var result = await sender.Send(new GetRolesListQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Assign role")]
    [EndpointDescription("Assigns role to user")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "AssignRolesPolicy")]
    [HttpPost("assign")]
    public async ValueTask<ActionResult<Response>> AssignAsync([FromBody] AssignRoleRequest request)
    {
        var result = await sender.Send(new AssignRoleCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Unassign roles")]
    [EndpointDescription("Unassign roles")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UnassignRolesPolicy")]
    [HttpPost("unassign")]
    public async ValueTask<ActionResult<Response>> UnassignAsync([FromBody] UnassignRolesRequest request)
    {
        var result = await sender.Send(new UnassignRolesCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Create role")]
    [EndpointDescription("Creates a role")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "CreateRolesPolicy")]
    [HttpPost]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> CreateAsync([FromBody] CreateRoleRequest request)
    {
        var result = await sender.Send(new CreateRoleCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Delete role")]
    [EndpointDescription("Deletes role")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "DeleteRolesPolicy")]
    [HttpPut("{id:guid}")]
    public async ValueTask<ActionResult<Response>> UpdateAsync([FromBody] UpdateRoleRequest request)
    {
        var result = await sender.Send(new UpdateRoleCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Delete role")]
    [EndpointDescription("Deletes role")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "DeleteRolesPolicy")]
    [HttpDelete("{id:guid}")]
    public async ValueTask<ActionResult<Response>> DeleteAsync(Guid id)
    {
        var result = await sender.Send(new DeleteRoleCommand(id));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}