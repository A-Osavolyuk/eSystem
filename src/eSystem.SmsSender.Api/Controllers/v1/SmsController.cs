using eSystem.SmsSender.Api.Interfaces;

namespace eSystem.SmsSender.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class SmsController(ISmsService smsService) : ControllerBase
{
    private readonly ISmsService _smsService = smsService;
}