using System.Security.Claims;
using System.Text.Json;
using eSecurity.Client.Security.Authentication;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Cookies;
using eSecurity.Core.Security.Cookies.Constants;
using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSystem.Core.Common.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        return Ok(HttpResponseBuilder.Create().Succeeded().Build());
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
        return Ok(HttpResponseBuilder.Create().Succeeded().Build());
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] TokenIdentity tokenIdentity)
    {
        _tokenProvider.IdToken = tokenIdentity.IdToken;

        var authenticationResult = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!authenticationResult.Succeeded)
            return Unauthorized(HttpResponseBuilder.Create().Failed().WithMessage("Failed authentication.").Build());

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            authenticationResult.Principal,
            authenticationResult.Properties
        );

        var cookieOptions = new CookieOptions()
        {
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            MaxAge = TimeSpan.FromDays(30)
        };
            
        HttpContext.Response.Cookies.Append(DefaultCookies.RefreshToken, tokenIdentity.RefreshToken, cookieOptions);

        var signInIdentity = new SignIdentity()
        {
            Scheme = authenticationResult.Ticket.AuthenticationScheme,
            Claims = authenticationResult.Principal.Claims.Select(claim => new ClaimValue()
            {
                Type = claim.Type,
                Value = claim.Value
            }).ToList()
        };

        return Ok(HttpResponseBuilder.Create().Succeeded().WithResult(signInIdentity).Build());
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(DefaultCookies.Session);

        return Ok(HttpResponseBuilder.Create().Succeeded().Build());
    }
}