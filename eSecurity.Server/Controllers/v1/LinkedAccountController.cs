using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.LinkedAccounts.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class LinkedAccountController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Disconnect linked account")]
    [EndpointDescription("Disconnect linked account")]
    [ProducesResponseType(200)]
    [HttpPost("disconnect")]
    [Authorize]
    public async ValueTask<IActionResult> DisconnectAsync([FromBody] DisconnectLinkedAccountRequest request)
    {
        var result = await _sender.Send(new DisconnectLinkedAccountCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}