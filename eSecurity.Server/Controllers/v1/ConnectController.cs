using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Connect.Commands;
using eSecurity.Server.Features.Connect.Queries;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Server.Controllers.v1;

[Route("v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class ConnectController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("OpenId configuration")]
    [EndpointDescription("OpenId configuration")]
    [ProducesResponseType(200)]
    [HttpGet(".well-known/openid-configuration")]
    public async ValueTask<IActionResult> OpenIdConfigurationAsync()
    {
        var result = await _sender.Send(new GetOpenidConfigurationQuery());
        return Ok(result.Value);
    }
    
    [EndpointSummary("Json Web Key")]
    [EndpointDescription("Json Web Key")]
    [ProducesResponseType(200)]
    [HttpGet(".well-known/jwks.json")]
    public async ValueTask<IActionResult> JsonWebKeyAsync()
    {
        var result = await _sender.Send(new GetJwksQuery());
        return Ok(result.Value);
    }

    [EndpointSummary("Token")]
    [EndpointDescription("Token")]
    [ProducesResponseType(200)]
    [HttpPost("token")]
    public async ValueTask<IActionResult> TokenAsync([FromBody] TokenRequest request)
    {
        var result = await _sender.Send(new TokenCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Authorize")]
    [EndpointDescription("Authorize")]
    [ProducesResponseType(200)]
    [HttpPost("authorize")]
    public async ValueTask<IActionResult> AuthorizeAsync([FromBody] AuthorizeRequest request)
    {
        var result = await _sender.Send(new AuthorizeCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Logout")]
    [EndpointDescription("Logout")]
    [ProducesResponseType(200)]
    [HttpPost("logout")]
    public async ValueTask<IActionResult> LogoutAsync([FromBody] LogoutRequest request)
    {
        var result = await _sender.Send(new LogoutCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}