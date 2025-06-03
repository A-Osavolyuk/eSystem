using eShop.Auth.Api.Features.Permissions.Queries;
using eShop.Auth.Api.Features.Users.Commands;
using eShop.Domain.Requests.API.Auth;

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
    [HttpGet]
    public async ValueTask<ActionResult<Response>> GetPermissionsAsync()
    {
        var result = await sender.Send(new GetPermissionsQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Grant permissions")]
    [EndpointDescription("Grant permission")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "GrantPermissionsPolicy")]
    [HttpPost("grant")]
    public async ValueTask<ActionResult<Response>> GrantAsync([FromBody] GrantPermissionRequest request)
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
    [HttpPost("revoke")]
    public async ValueTask<ActionResult<Response>> RevokeAsync(
        [FromBody] RevokePermissionRequest request)
    {
        var result = await sender.Send(new RevokePermissionCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}