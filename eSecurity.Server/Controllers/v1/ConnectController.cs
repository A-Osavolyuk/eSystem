using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Connect.Commands;
using eSecurity.Server.Features.Connect.Queries;
using eSystem.Core.Common.Http.Constants;

namespace eSecurity.Server.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Produces(ContentTypes.Application.Json)]
[Route("v{version:apiVersion}/[controller]")]
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
        return ResultHandler.Handle(result);
    }

    [EndpointSummary("Json Web Key")]
    [EndpointDescription("Json Web Key")]
    [ProducesResponseType(200)]
    [HttpGet(".well-known/jwks.json")]
    public async ValueTask<IActionResult> JsonWebKeyAsync()
    {
        var result = await _sender.Send(new GetJwksQuery());
        return ResultHandler.Handle(result);
    }

    [EndpointSummary("Client info")]
    [EndpointDescription("Client info")]
    [ProducesResponseType(200)]
    [HttpGet("clients/{clientId}")]
    public async ValueTask<IActionResult> GetClientInfoAsync(string clientId)
    {
        var result = await _sender.Send(new GetClientInfoQuery(clientId));
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Userinfo")]
    [EndpointDescription("Userinfo")]
    [ProducesResponseType(200)]
    [HttpGet("userinfo")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async ValueTask<IActionResult> GetUserInfoAsync()
    {
        if (Request.HasFormContentType)
        {
            const string description = "GET cannot have form body";
            Response.Headers.Append(HeaderTypes.WwwAuthenticate,
                $"Bearer error=\"{Errors.OAuth.InvalidRequest}\", error_description=\"{description}\"");
            
            return BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = description
            });
        }

        
        var result = await _sender.Send(new GetUserInfoQuery());
        return ResultHandler.Handle(result);
    }
    
    [EndpointSummary("Userinfo")]
    [EndpointDescription("Userinfo")]
    [ProducesResponseType(200)]
    [HttpPost("userinfo")]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> PostUserInfoAsync([FromForm] UserInfoRequest request)
    {
        var result = await _sender.Send(new GetUserInfoQuery(request.AccessToken));
        return ResultHandler.Handle(result);
    }

    [EndpointSummary("Grant consents")]
    [EndpointDescription("Grant consents")]
    [ProducesResponseType(200)]
    [HttpPost("consents/grant")]
    public async ValueTask<IActionResult> GrantConsentsAsync([FromBody] GrantConsentsRequest request)
    {
        var result = await _sender.Send(new GrantConsentsCommand(request));
        return ResultHandler.Handle(result);
    }

    [EndpointSummary("Token")]
    [EndpointDescription("Token")]
    [ProducesResponseType(200)]
    [HttpPost("token")]
    [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> TokenAsync([FromForm] TokenRequest request)
    {
        var result = await _sender.Send(new TokenCommand(request));
        return ResultHandler.Handle(result);
    }

    [EndpointSummary("Revocation")]
    [EndpointDescription("Revocation")]
    [ProducesResponseType(200)]
    [HttpPost("revocation")]
    [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> RevokeAsync([FromForm] RevocationRequest request)
    {
        var result = await _sender.Send(new RevokeCommand(request));
        return ResultHandler.Handle(result);
    }

    [EndpointSummary("Introspection")]
    [EndpointDescription("Introspection")]
    [ProducesResponseType(200)]
    [HttpPost("introspection")]
    [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> IntrospectionAsync([FromForm] IntrospectionRequest request)
    {
        var result = await _sender.Send(new IntrospectionCommand(request));
        return ResultHandler.Handle(result);
    }

    [EndpointSummary("Authorize")]
    [EndpointDescription("Authorize")]
    [ProducesResponseType(200)]
    [HttpPost("authorize")]
    public async ValueTask<IActionResult> AuthorizeAsync([FromBody] AuthorizeRequest request)
    {
        var result = await _sender.Send(new AuthorizeCommand(request));
        return ResultHandler.Handle(result);
    }

    [EndpointSummary("Logout")]
    [EndpointDescription("Logout")]
    [ProducesResponseType(200)]
    [HttpPost("logout")]
    public async ValueTask<IActionResult> LogoutAsync([FromBody] LogoutRequest request)
    {
        var result = await _sender.Send(new LogoutCommand(request));
        return ResultHandler.Handle(result);
    }
}