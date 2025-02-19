using eShop.EmailSender.Api.Requests;
using Response = eShop.Domain.Common.Api.Response;

namespace eShop.EmailSender.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class EmailController(IEmailService emailService) : ControllerBase
{
    [HttpPost("send-message")]
    [EndpointSummary("Send email message")]
    [EndpointDescription("Sends email message")]
    [ProducesResponseType(200)]
    public async ValueTask<ActionResult<Response>> SendMessageAsync([FromBody] SendMessageRequest request)
    {
        await emailService.SendMessageAsync(request.HtmlBody, request.Options);

        return Ok(new ResponseBuilder().Succeeded().WithMessage("Message sent successfully").Build());
    }
}