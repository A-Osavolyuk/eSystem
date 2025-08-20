using eShop.Auth.Api.Features.Permissions.Commands;
using eShop.Auth.Api.Features.Permissions.Queries;
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
    [HttpGet]
    public async ValueTask<IActionResult> GetPermissionsAsync()
    {
        var result = await sender.Send(new GetPermissionsQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Grant permissions")]
    [EndpointDescription("Grant permission")]
    [ProducesResponseType(200)]
    [HttpPost("grant")]
    public async ValueTask<IActionResult> GrantAsync([FromBody] GrantPermissionRequest request)
    {
        var result = await sender.Send(new GrantPermissionCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Revoke permission")]
    [EndpointDescription("Revoke permission")]
    [ProducesResponseType(200)]
    [HttpPost("revoke")]
    public async ValueTask<IActionResult> RevokeAsync(
        [FromBody] RevokePermissionRequest request)
    {
        var result = await sender.Send(new RevokePermissionCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}