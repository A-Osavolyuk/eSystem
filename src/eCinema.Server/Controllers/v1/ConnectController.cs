using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCinema.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class ConnectController : ControllerBase
{
    [HttpGet("frontchannel-logout")]
    [EndpointSummary("Front-Channel Logout Endpoint")]
    public IActionResult FrontChannelLogout()
    {
        return SignOut(
            new AuthenticationProperties(),
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }
}