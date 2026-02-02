using eCinema.Server.Hubs;
using eCinema.Server.Security.Authentication.OpenIdConnect.Session;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Logout;
using eSystem.Core.Security.Authentication.OpenIdConnect.Token.Validation;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace eCinema.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class ConnectController(
    IHubContext<AuthenticationHub> hub,
    ITokenValidator tokenValidator,
    ISessionManager sessionManager) : ControllerBase
{
    private readonly IHubContext<AuthenticationHub> _hub = hub;
    private readonly ITokenValidator _tokenValidator = tokenValidator;
    private readonly ISessionManager _sessionManager = sessionManager;

    [HttpGet("frontchannel-logout")]
    [EndpointSummary("Front-Channel Logout Endpoint")]
    public async Task<IActionResult> FrontChannelLogout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
        catch (Exception)
        {
            return Ok();
        }
    }

    [HttpPost("backchannel-logout")]
    [EndpointSummary("Back-Channel Logout Endpoint")]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async Task<IActionResult> BackChannelLogoutAsync([FromForm] BackChannelLogoutRequest request)
    {
        var tokenResult = await _tokenValidator.ValidateAsync(request.LogoutToken);
        if (!tokenResult.IsValid || tokenResult.ClaimsPrincipal is null) return Ok();
        
        var sid = tokenResult.ClaimsPrincipal.Claims.First(x => x.Type == AppClaimTypes.Sid).Value;
        var session = await _sessionManager.FindBySidAsync(sid);
        if (session is null) return Ok();
        
        await _hub.Clients.User(session.UserId).SendAsync("Logout");
        await _sessionManager.DeleteAsync(session);
        
        return Ok();
    }
}