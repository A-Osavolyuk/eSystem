using eShop.Auth.Api.Features.Lockout.Commands;
using eShop.Auth.Api.Features.Lockout.Query;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class LockoutController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get all reasons")]
    [EndpointDescription("Get all reasons")]
    [ProducesResponseType(200)]
    [HttpPost("reasons")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetReasonsAsync()
    {
        var result = await sender.Send(new GetReasonsQuery());
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Lockout")]
    [EndpointDescription("Lockout")]
    [ProducesResponseType(200)]
    [HttpPost("lockout")]
    public async ValueTask<IActionResult> LockoutAsync([FromBody] LockoutRequest request)
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
    public async ValueTask<IActionResult> UnlockAsync([FromBody] UnlockRequest request)
    {
        var result = await sender.Send(new UnlockCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}