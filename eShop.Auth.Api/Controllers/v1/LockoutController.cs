using eShop.Auth.Api.Features.Lockout.Commands;
using eShop.Auth.Api.Features.Lockout.Queries;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class LockoutController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get lockout state")]
    [EndpointDescription("Get lockout state")]
    [ProducesResponseType(200)]
    [HttpGet("state/{id:guid}")]
    [Authorize(Policy = "ReadUsersPolicy")]
    public async ValueTask<ActionResult<Response>> GetStateAsync(Guid id)
    {
        var result = await sender.Send(new GetLockoutStateQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Lockout")]
    [EndpointDescription("Lockout")]
    [ProducesResponseType(200)]
    [HttpPost("lockout")]
    [Authorize(Policy = "LockoutUsersPolicy")]
    public async ValueTask<ActionResult<Response>> LockoutAsync([FromBody] LockoutRequest request)
    {
        var result = await sender.Send(new LockoutCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Unlock")]
    [EndpointDescription("Unlock")]
    [ProducesResponseType(200)]
    [HttpPost("unlock")]
    [Authorize(Policy = "UnlockUsersPolicy")]
    public async ValueTask<ActionResult<Response>> UnlockAsync([FromBody] UnlockRequest request)
    {
        var result = await sender.Send(new UnlockCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}