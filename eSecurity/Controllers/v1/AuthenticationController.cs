using System.Security.Claims;
using System.Text.Json;
using eSecurity.Security.Authentication.Jwt;
using eSecurity.Security.Authentication.Odic.Session;
using eSystem.Core.Common.Http;
using eSystem.Core.Security.Cookies;
using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eSecurity.Controllers.v1;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(
    TokenProvider tokenProvider,
    IProtectorFactory protectorFactory) : ControllerBase
{
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly IProtectorFactory protectorFactory = protectorFactory;
    
    [HttpPost("authorize")]
    public IActionResult Authorize([FromBody] SessionCookie cookie)
    {
        var cookieJson = JsonSerializer.Serialize(cookie);
        var protector = protectorFactory.Create(ProtectionPurposes.Session);
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
        return Ok(HttpResponseBuilder.Create().Succeeded().Build());
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync()
    {
        //TODO: Get user claims
        
        var principal = new ClaimsPrincipal();
        var properties = new AuthenticationProperties() { IsPersistent = true, };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        
        return Ok(Result.Success());
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(DefaultCookies.Session);

        return Ok(HttpResponseBuilder.Create().Succeeded().Build());
    }
}