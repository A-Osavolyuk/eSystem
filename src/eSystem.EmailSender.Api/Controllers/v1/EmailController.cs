using eSystem.EmailSender.Api.Requests;

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
        return Ok();
    }
}