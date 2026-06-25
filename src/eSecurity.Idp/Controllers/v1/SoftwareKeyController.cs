using eSecurity.Idp.Features.Passkeys;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Idp.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/software-key")]
public class SoftwareKeyController(IMediator mediator) : ControllerBase
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
    
    [EndpointSummary("Create software key")]
    [EndpointDescription("Create software key")]
    [ProducesResponseType(200)]
    [HttpPost("create")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> CreateAsync([FromBody] CreateSoftwareKeyCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Remove software key")]
    [EndpointDescription("Remove software key")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemoveSoftwareKeyCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Change software key display name")]
    [EndpointDescription("Change software key display name")]
    [ProducesResponseType(200)]
    [HttpPost("change-name")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeNameAsync([FromBody] ChangeSoftwareKeyNameCommand command)
    {
        var result = await _mediator.Send(command);
        return HttpContext.HandleResult(result);
    }
}