using System.Security.Claims;
using System.Text.Json;
using eSecurity.Client.Security.Authentication;
using eSecurity.Client.Security.Cookies;
using eSecurity.Client.Security.Cookies.Constants;
using eSecurity.Client.Security.Cryptography.Protection.Constants;
using eSecurity.Core.Common.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace eSecurity.Client.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IDataProtectionProvider protectionProvider) : ControllerBase
{
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;

    [HttpPost("authorize")]
    public IActionResult Authorize([FromBody] SessionCookie cookie)
    {
        var cookieJson = JsonSerializer.Serialize(cookie);
        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Session);
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
        return Ok(Results.Ok());
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(DefaultCookies.Session);

        return Ok(Results.Ok());
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignIdentity identity)
    {
        var claims = identity.Claims.Select(x => new Claim(x.Type, x.Value));
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var authenticationProperties = new AuthenticationProperties { IsPersistent = true };

        var cookie = new AuthenticationMetadata()
        {
            Tokens = identity.Tokens
        };
        
        var cookieJson = JsonSerializer.Serialize(cookie);
        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Token);
        var protectedCookie = protector.Protect(cookieJson);
        var cookieOptions = new CookieOptions()
        {
            Domain = "localhost",
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            MaxAge = TimeSpan.FromDays(30)
        };
        
        Response.Cookies.Append(DefaultCookies.Payload, protectedCookie, cookieOptions);
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authenticationProperties
        );

        return Ok(Results.Ok());
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(DefaultCookies.Payload);

        return Ok(Results.Ok());
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] AuthenticationMetadata metadata)
    {
        var cookieJson = JsonSerializer.Serialize(metadata);
        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Token);
        var protectedCookie = protector.Protect(cookieJson);
        var cookieOptions = new CookieOptions()
        {
            Domain = "localhost",
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            MaxAge = TimeSpan.FromDays(30)
        };

        Response.Cookies.Append(DefaultCookies.Payload, protectedCookie, cookieOptions);
        return Ok(Results.Ok());
    }
}