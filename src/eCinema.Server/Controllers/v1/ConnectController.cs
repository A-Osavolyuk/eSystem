using eCinema.Server.Features.Connect.Commands;
using eSystem.Core.Http.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCinema.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ConnectController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("authorize")]
    [EndpointSummary("Authorize")]
    public async ValueTask<IActionResult> Authorize()
    {
        var result = await _sender.Send(new AuthorizeCommand());
        return HttpContext.HandleResult(result);
    }

    [HttpGet("callback")]
    [EndpointSummary("Callback")]
    public async ValueTask<IActionResult> Callback(
        [FromQuery(Name = "code")] string? code,
        [FromQuery(Name = "state")] string? state,
        [FromQuery(Name = "error")] string? error,
        [FromQuery(Name = "error_description")]
        string? description)
    {
        var result = await _sender.Send(new AuthorizeCallbackCommand()
        {
            Code = code,
            State = state,
            Error = error,
            ErrorDescription = description
        });
        
        return HttpContext.HandleResult(result);
    }
}