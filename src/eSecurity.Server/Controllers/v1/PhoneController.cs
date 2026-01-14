using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Phone;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class PhoneController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Verify phone number")]
    [EndpointDescription("Verify phone number")]
    [ProducesResponseType(200)]
    [HttpPost("verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyAsync([FromBody] VerifyPhoneNumberRequest request)
    {
        var result = await _sender.Send(new VerifyPhoneNumberCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check phone number")]
    [EndpointDescription("Check phone number")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckPhoneNumberRequest request)
    {
        var result = await _sender.Send(new CheckPhoneNumberCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Add phone number")]
    [EndpointDescription("Add phone number")]
    [ProducesResponseType(200)]
    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> AddAsync([FromBody] AddPhoneNumberRequest request)
    {
        var result = await _sender.Send(new AddPhoneNumberCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Change phone number")]
    [EndpointDescription("Change phone number")]
    [ProducesResponseType(200)]
    [HttpPost("change")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeAsync([FromBody] ChangePhoneNumberRequest request)
    {
        var result = await _sender.Send(new ChangePhoneNumberCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Reset phone number")]
    [EndpointDescription("Reset phone number")]
    [ProducesResponseType(200)]
    [HttpPost("reset")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ResetAsync([FromBody] ResetPhoneNumberRequest request)
    {
        var result = await _sender.Send(new ResetPhoneNumberCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Remove phone number")]
    [EndpointDescription("Remove phone number")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemovePhoneNumberRequest request)
    {
        var result = await _sender.Send(new RemovePhoneNumberCommand(request));
        return HttpContext.HandleResult(result);
    }
}