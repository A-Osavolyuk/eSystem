using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Passkeys.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class PasskeyController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Generate public key credential creation options")]
    [EndpointDescription("Generate public key credential creation options")]
    [ProducesResponseType(200)]
    [HttpPost("options/creation")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GenerateCreationOptionsAsync(
        [FromBody] GenerateCreationOptionsRequest request)
    {
        var result = await _sender.Send(new GenerateCreationOptionsCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Generate public key credential request options")]
    [EndpointDescription("Generate public key credential request options")]
    [ProducesResponseType(200)]
    [HttpPost("options/request")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GenerateRequestOptionsAsync([FromBody] GenerateRequestOptionsRequest request)
    {
        var result = await _sender.Send(new GenerateRequestOptionsCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Create passkey")]
    [EndpointDescription("Create passkey")]
    [ProducesResponseType(200)]
    [HttpPost("create")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> CreateAsync([FromBody] CreatePasskeyRequest request)
    {
        var result = await _sender.Send(new CreatePasskeyCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Remove passkey")]
    [EndpointDescription("Remove passkey")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemovePasskeyRequest request)
    {
        var result = await _sender.Send(new RemovePasskeyCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Change passkey display name")]
    [EndpointDescription("Change passkey display name")]
    [ProducesResponseType(200)]
    [HttpPost("change-name")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeNameAsync([FromBody] ChangePasskeyNameRequest request)
    {
        var result = await _sender.Send(new ChangePasskeyNameCommand(request));
        return HttpContext.HandleResult(result);
    }
}