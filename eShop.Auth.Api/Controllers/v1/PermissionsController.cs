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
public class PermissionsController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

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

    [EndpointSummary("Revoke permission")]
    [EndpointDescription("Revoke permission")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "RevokePermissionPolicy")]
    [HttpPost("revoke-permission")]
    public async ValueTask<ActionResult<Response>> RevokePermissionAsync(
        [FromBody] RevokePermissionRequest request)
    {
        var result = await sender.Send(new RevokePermissionCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}