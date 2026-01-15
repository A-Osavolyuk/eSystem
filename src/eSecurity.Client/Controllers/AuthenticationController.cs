using System.Security.Claims;
using System.Text.Json;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Cookies;
using eSystem.Core.Security.Cookies.Constants;
using eSystem.Core.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace eSecurity.Client.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(
    IDataProtectionProvider protectionProvider,
    TokenProvider tokenProvider) : ControllerBase
{
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly TokenProvider _tokenProvider = tokenProvider;

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

    [HttpPost("refresh")]
    public IActionResult Authorize([FromBody] string refreshToken)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromDays(30)
        };

        Response.Cookies.Append(DefaultCookies.RefreshToken, refreshToken, cookieOptions);
        return Ok(Results.Ok());
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignIdentity identity)
    {
        var claims = identity.Claims.Select(x => new Claim(x.Type, x.Value));
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var authenticationProperties = new AuthenticationProperties() { IsPersistent = true };
        
        await HttpContext.SignInAsync(
            identity.Scheme,
            claimsPrincipal,
            authenticationProperties
        );

        var cookieOptions = new CookieOptions()
        {
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            MaxAge = TimeSpan.FromDays(30)
        };
            
        HttpContext.Response.Cookies.Append(DefaultCookies.RefreshToken, identity.RefreshToken, cookieOptions);

        return Ok(Results.Ok());
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(DefaultCookies.RefreshToken);

        return Ok(Results.Ok());
    }
}