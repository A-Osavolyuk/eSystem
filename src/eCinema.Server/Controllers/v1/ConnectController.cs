using eCinema.Server.Hubs;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Logout;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace eCinema.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class ConnectController(IHubContext<AuthenticationHub> hub) : ControllerBase
{
    private readonly IHubContext<AuthenticationHub> _hub = hub;

    [HttpGet("frontchannel-logout")]
    [EndpointSummary("Front-Channel Logout Endpoint")]
    public IActionResult FrontChannelLogout()
    {
        return SignOut(
            new AuthenticationProperties(),
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }

    [HttpPost("backchannel-logout")]
    [EndpointSummary("Back-Channel Logout Endpoint")]
    [Consumes(ContentTypes.Application.XwwwFormUrlEncoded)]
    public async Task<IActionResult> BackChannelLogoutAsync([FromForm] BackChannelLogoutRequest request)
    {
        return Ok();
    }
}