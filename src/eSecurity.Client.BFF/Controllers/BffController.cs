using eSecurity.Client.BFF.Security.Cors;
using eSystem.Core.Server.Bff;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.BFF.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
[EnableCors(PolicyName = CorsPolicies.SpaOnly)]
public sealed class BffController(IOptions<BffOptions> bffOptions) : ControllerBase
{
    private readonly BffOptions _bffOptions = bffOptions.Value;

    [HttpGet("login")]
    [EndpointSummary("Login")]
    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties
        {
            IsPersistent = true,
            RedirectUri = _bffOptions.FrontendUri
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