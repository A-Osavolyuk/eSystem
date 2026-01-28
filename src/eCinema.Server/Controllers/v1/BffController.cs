using eCinema.Server.Security.Cors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace eCinema.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[EnableCors(CorsPolicies.SpaOnly)]
public class BffController() : ControllerBase
{
    [HttpGet("login")]
    [EndpointSummary("Login")]
    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties
        {
            IsPersistent = true,
            RedirectUri = "https://localhost:6511"
        });
    }

    [HttpGet("logout")]
    [EndpointSummary("Logout")]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties(),
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }
}