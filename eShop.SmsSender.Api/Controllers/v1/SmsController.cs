using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Sms;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class SmsController(ISmsService smsService) : ControllerBase
{
    private readonly ISmsService smsService = smsService;
}