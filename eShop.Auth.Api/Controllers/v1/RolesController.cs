using eShop.Auth.Api.Features.Roles.Commands;
using eShop.Auth.Api.Features.Roles.Queries;
using eShop.Domain.Common.Http;
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
    [HttpGet]
    public async ValueTask<IActionResult> GetRolesAsync()
    {
        var result = await sender.Send(new GetRolesQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Assign role")]
    [EndpointDescription("Assigns role to user")]
    [ProducesResponseType(200)]
    [HttpPost("assign")]
    public async ValueTask<IActionResult> AssignAsync([FromBody] AssignRoleRequest request)
    {
        var result = await sender.Send(new AssignRoleCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Unassign roles")]
    [EndpointDescription("Unassign roles")]
    [ProducesResponseType(200)]
    [HttpPost("unassign")]
    public async ValueTask<IActionResult> UnassignAsync([FromBody] UnassignRolesRequest request)
    {
        var result = await sender.Send(new UnassignRolesCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Create role")]
    [EndpointDescription("Creates a role")]
    [ProducesResponseType(200)]
    [HttpPost]
    [ValidationFilter]
    public async ValueTask<IActionResult> CreateAsync([FromBody] CreateRoleRequest request)
    {
        var result = await sender.Send(new CreateRoleCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Delete role")]
    [EndpointDescription("Deletes role")]
    [ProducesResponseType(200)]
    [HttpPut("{id:guid}")]
    public async ValueTask<IActionResult> UpdateAsync([FromBody] UpdateRoleRequest request)
    {
        var result = await sender.Send(new UpdateRoleCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Delete role")]
    [EndpointDescription("Deletes role")]
    [ProducesResponseType(200)]
    [HttpDelete("{id:guid}")]
    public async ValueTask<IActionResult> DeleteAsync(Guid id)
    {
        var result = await sender.Send(new DeleteRoleCommand(id));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}