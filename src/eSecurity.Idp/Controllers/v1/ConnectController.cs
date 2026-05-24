using eSecurity.Idp.Features.Connect.Commands;
using eSecurity.Idp.Features.Connect.Queries;
using eSecurity.Idp.Security.Authentication.EndSession;
using eSecurity.Idp.Security.Authorization;
using eSecurity.Idp.Security.Authorization.Constants;
using eSystem.Core.Enums;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.User;
using eSystem.Core.Server.Security.Authorization.OAuth.DeviceAuthorization;

namespace eSecurity.Idp.Controllers.v1;

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
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("OAuth authorization server")]
    [EndpointDescription("OAuth authorization server")]
    [ProducesResponseType(200)]
    [HttpGet(".well-known/oauth-authorization-server")]
    public async ValueTask<IActionResult> OAuthAuthorizationServerAsync()
    {
        var result = await _sender.Send(new GetOAuthAuthorizationServerQuery());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Json Web Key")]
    [EndpointDescription("Json Web Key")]
    [ProducesResponseType(200)]
    [HttpGet(".well-known/jwks.json")]
    public async ValueTask<IActionResult> JsonWebKeyAsync()
    {
        var result = await _sender.Send(new GetJwksQuery());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Client info")]
    [EndpointDescription("Client info")]
    [ProducesResponseType(200)]
    [HttpGet("clients/{clientId}")]
    public async ValueTask<IActionResult> GetClientInfoAsync(string clientId)
    {
        var result = await _sender.Send(new GetClientInfoQuery(clientId));
        return HttpContext.HandleResult(result);
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
                $"Bearer error=\"{ErrorCode.InvalidRequest.GetString()}\", error_description=\"{description}\"");

            return BadRequest(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = description
            });
        }


        var result = await _sender.Send(new GetUserInfoQuery());
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Userinfo")]
    [EndpointDescription("Userinfo")]
    [ProducesResponseType(200)]
    [HttpPost("userinfo")]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> PostUserInfoAsync([FromForm] UserInfoRequest request)
    {
        var result = await _sender.Send(new GetUserInfoQuery(request.AccessToken));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Token")]
    [EndpointDescription("Token")]
    [ProducesResponseType(200)]
    [HttpPost("token")]
    [Authorize(Policy = AuthorizationPolicies.TokenAuthorization)]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> TokenAsync([FromForm] IFormCollection form)
    {
        var result = await _sender.Send(new TokenCommand(form));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Revocation")]
    [EndpointDescription("Revocation")]
    [ProducesResponseType(200)]
    [HttpPost("revocation")]
    [Authorize(Policy = AuthorizationPolicies.BasicAuthorization)]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> RevokeAsync([FromForm] IFormCollection request)
    {
        var result = await _sender.Send(new RevokeCommand(request));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Introspection")]
    [EndpointDescription("Introspection")]
    [ProducesResponseType(200)]
    [HttpPost("introspection")]
    [Authorize(Policy = AuthorizationPolicies.BasicAuthorization)]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> IntrospectionAsync([FromForm] IFormCollection request)
    {
        var result = await _sender.Send(new IntrospectionCommand(request));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Authorize")]
    [EndpointDescription("Authorize")]
    [ProducesResponseType(200)]
    [HttpGet("authorize")]
    public async ValueTask<IActionResult> AuthorizeAsync([FromQuery] AuthorizationRequest request)
    {
        var result = await _sender.Send(new AuthorizationCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Pushed authorization request")]
    [EndpointDescription("Pushed authorization request")]
    [ProducesResponseType(200)]
    [HttpPost("par")]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async ValueTask<IActionResult> PushedAuthorizationRequestAsync([FromForm] IFormCollection form)
    {
        var result = await _sender.Send(new PushedAuthorizationRequestCommand(form));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("End session")]
    [EndpointDescription("End session")]
    [ProducesResponseType(200)]
    [HttpGet("end-session")]
    public async ValueTask<IActionResult> EndSessionAsync([FromQuery] EndSessionRequest request)
    {
        var result = await _sender.Send(new EndSessionCommand(request));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Confirm end session")]
    [EndpointDescription("Confirm end session")]
    [ProducesResponseType(200)]
    [HttpGet("confirm-end-session")]
    public async ValueTask<IActionResult> ConfirmEndSessionAsync([FromQuery] ConfirmEndSessionRequest request)
    {
        var result = await _sender.Send(new ConfirmEndSessionCommand(request));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Device authorization")]
    [EndpointDescription("Device authorization")]
    [ProducesResponseType(200)]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    [HttpPost("device-authorization")]
    public async ValueTask<IActionResult> DeviceAuthorizationAsync([FromForm] DeviceAuthorizationRequest request)
    {
        var result = await _sender.Send(new DeviceAuthorizationCommand(request));
        return HttpContext.HandleResult(result);
    }

    [EndpointSummary("Backchannel authentication")]
    [EndpointDescription("Backchannel authentication")]
    [ProducesResponseType(200)]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    [HttpPost("backchannel-authentication")]
    public async ValueTask<IActionResult> BackchannelAuthenticationAsync(
        [FromForm] BackchannelAuthenticationRequest request)
    {
        var result = await _sender.Send(new BackchannelAuthenticationCommand(request));
        return HttpContext.HandleResult(result);
    }
}