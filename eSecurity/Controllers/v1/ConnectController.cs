using eSecurity.Features.ODIC.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class ConnectController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Token")]
    [EndpointDescription("Token")]
    [ProducesResponseType(200)]
    [HttpPost("token")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> TokenAsync([FromBody] TokenCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}