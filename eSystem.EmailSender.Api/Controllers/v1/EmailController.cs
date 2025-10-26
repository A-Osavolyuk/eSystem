using eSystem.Core.Common.Http;
using eSystem.EmailSender.Api.Requests;
using HttpResponse = eSystem.Core.Common.Http.HttpResponse;

namespace eSystem.EmailSender.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class EmailController(IEmailService emailService) : ControllerBase
{
    [HttpPost("send-message")]
    [EndpointSummary("Send email message")]
    [EndpointDescription("Sends email message")]
    [ProducesResponseType(200)]
    public async ValueTask<ActionResult<HttpResponse>> SendMessageAsync([FromBody] SendMessageRequest request)
    {
        await emailService.SendMessageAsync(request.HtmlBody, request.Options);

        return Ok(new ResponseBuilder().Succeeded().WithMessage("Message sent successfully").Build());
    }
}