using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Verification.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VerificationController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Send code")]
    [EndpointDescription("Send code")]
    [ProducesResponseType(200)]
    [HttpPost("code/send")]
    public async ValueTask<IActionResult> SendCodeAsync([FromBody] SendCodeRequest request)
    {
        var result = await _sender.Send(new SendCodeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Resend code")]
    [EndpointDescription("Resend code")]
    [ProducesResponseType(200)]
    [HttpPost("code/resend")]
    public async ValueTask<IActionResult> ResendCodeAsync([FromBody] ResendCodeRequest request)
    {
        var result = await _sender.Send(new ResendCodeCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Verification request")]
    [EndpointDescription("Verification request")]
    [ProducesResponseType(200)]
    [HttpPost("request-verification")]
    public async ValueTask<IActionResult> VerificationRequestAsync([FromBody] VerificationRequest request)
    {
        var result = await _sender.Send(new VerificationCommand(request));
        return HttpContext.HandleResult(result);
    }
}