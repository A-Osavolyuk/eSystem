using eAccount.Infrastructure.Security.Authentication.SSO;
using eSystem.Core.Security.Authentication.Cookies;
using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eAccount.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SsoController(IProtectorFactory factory) : ControllerBase
{
    private readonly IProtectorFactory factory = factory;

    [HttpPost("authorize")]
    public IActionResult Authorize([FromBody] SessionCookie cookie)
    {
        var cookieJson = JsonSerializer.Serialize(cookie);
        var protector = factory.Create(ProtectionPurposes.Session);
        var protectedCookie = protector.Protect(cookieJson);
        var cookieOptions = new CookieOptions()
        {
            Domain = "localhost",
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = cookie.ExpiresAt.UtcDateTime,
            MaxAge = TimeSpan.FromDays(30)
        };

        Response.Cookies.Append(DefaultCookies.Session, protectedCookie, cookieOptions);
        return Ok(new ResponseBuilder().Succeeded().Build());
    }
}