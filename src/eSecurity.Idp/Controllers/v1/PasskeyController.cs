using eSecurity.Core.Requests;
using eSecurity.Idp.Features.Passkeys;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class PasskeyController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [EndpointSummary("Generate public key credential creation options")]
    [EndpointDescription("Generate public key credential creation options")]
    [ProducesResponseType(200)]
    [HttpPost("options/creation")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GenerateCreationOptionsAsync([FromBody] GenerateCreationOptionsCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Generate public key credential request options")]
    [EndpointDescription("Generate public key credential request options")]
    [ProducesResponseType(200)]
    [HttpPost("options/request")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GenerateRequestOptionsAsync([FromBody] GenerateRequestOptionsCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Create passkey")]
    [EndpointDescription("Create passkey")]
    [ProducesResponseType(200)]
    [HttpPost("create")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> CreateAsync([FromBody] CreatePasskeyCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Remove passkey")]
    [EndpointDescription("Remove passkey")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemovePasskeyCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Change passkey display name")]
    [EndpointDescription("Change passkey display name")]
    [ProducesResponseType(200)]
    [HttpPost("change-name")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeNameAsync([FromBody] ChangePasskeyNameCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
}