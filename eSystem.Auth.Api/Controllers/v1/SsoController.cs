using eSystem.Auth.Api.Features.SSO.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class SsoController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Authorize")]
    [EndpointDescription("Authorize")]
    [ProducesResponseType(200)]
    [HttpPost("authorize")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> AuthorizeAsync([FromBody] AuthorizeRequest request)
    {
        var result = await sender.Send(new AuthorizeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Token")]
    [EndpointDescription("Token")]
    [ProducesResponseType(200)]
    [HttpPost("token")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> TokenAsync([FromBody] TokenRequest request)
    {
        var result = await sender.Send(new TokenCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Sign-out")]
    [EndpointDescription("Sign-out")]
    [ProducesResponseType(200)]
    [HttpPost("sign-out")]
    [Authorize]
    public async ValueTask<IActionResult> SignOutAsync([FromBody] SignOutRequest request)
    {
        var result = await sender.Send(new SignOutCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}