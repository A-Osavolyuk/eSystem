using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Email.Commands;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
public class EmailController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Verify email")]
    [EndpointDescription("Verify email")]
    [ProducesResponseType(200)]
    [HttpPost("verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyAsync([FromBody] VerifyEmailRequest request)
    {
        var result = await _sender.Send(new VerifyEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Check email")]
    [EndpointDescription("Check email")]
    [ProducesResponseType(200)]
    [HttpPost("check")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CheckAsync([FromBody] CheckEmailRequest request)
    {
        var result = await _sender.Send(new CheckEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Add email")]
    [EndpointDescription("Add email")]
    [ProducesResponseType(200)]
    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> AddAsync([FromBody] AddEmailRequest request)
    {
        var result = await _sender.Send(new AddEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Change email")]
    [EndpointDescription("Change email")]
    [ProducesResponseType(200)]
    [HttpPost("change")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ChangeAsync([FromBody] ChangeEmailRequest request)
    {
        var result = await _sender.Send(new ChangeEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Reset email")]
    [EndpointDescription("Reset email")]
    [ProducesResponseType(200)]
    [HttpPost("reset")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ResetAsync([FromBody] ResetEmailRequest request)
    {
        var result = await _sender.Send(new ResetEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Remove email")]
    [EndpointDescription("Remove email")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemoveEmailRequest request)
    {
        var result = await _sender.Send(new RemoveEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Manage email")]
    [EndpointDescription("Manage email")]
    [ProducesResponseType(200)]
    [HttpPost("manage")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> ManageAsync([FromBody] ManageEmailRequest request)
    {
        var result = await _sender.Send(new ManageEmailCommand(request));
        return HttpContext.HandleResult(result);
    }
}