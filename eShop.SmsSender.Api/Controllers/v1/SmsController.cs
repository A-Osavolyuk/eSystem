using eShop.Domain.Requests.Api.Sms;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class SmsController(ISmsService smsService) : ControllerBase
{
    private readonly ISmsService smsService = smsService;

    [HttpPost("send-single-sms")]
    [EndpointSummary("Send single SMS")]
    [EndpointDescription("Sends SMS message")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<Response>> SendSingleMessageAsync([FromBody] SingleMessageRequest request)
    {
        var response = await smsService.SendSingleMessage(request);

        return response.IsSucceeded
            ? Ok(new ResponseBuilder().Succeeded().WithMessage(response.Message).Build())
            : StatusCode((int)response.StatusCode,
                new ResponseBuilder().Failed().WithMessage(response.Message).Build());
    }
}