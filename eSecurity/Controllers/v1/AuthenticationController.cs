using System.Text.Json;
using eSecurity.Common.Responses;
using eSecurity.Security.Authentication.Jwt;
using eSecurity.Security.Authentication.Odic;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Cookies;
using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SignInRequest = eSecurity.Common.Requests.SignInRequest;

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
    
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
        };
        
        Response.Cookies.Append(DefaultCookies.RefreshToken, request.RefreshToken, cookieOptions);
        return Ok(HttpResponseBuilder.Create().Succeeded().Build());
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
        };

        tokenProvider.AccessToken = request.AccessToken;
        Response.Cookies.Append(DefaultCookies.RefreshToken, request.RefreshToken, cookieOptions);

        var authenticateResult = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return Unauthorized(HttpResponseBuilder.Create()
                .Failed()
                .Build());
        }

        var principal = authenticateResult.Principal!;
        var properties = new AuthenticationProperties() { IsPersistent = true, };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

        var identity = new Identity
        {
            Claims = principal.Claims.ToDictionary(x => x.Type, y => y.Value),
            Scheme = CookieAuthenticationDefaults.AuthenticationScheme
        };

        var response = new SignInResponse()
        {
            Identity = identity,
            AccessToken = tokenProvider.AccessToken
        };

        return Ok(HttpResponseBuilder.Create().Succeeded().WithResult(response).Build());
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        Response.Cookies.Delete(DefaultCookies.RefreshToken);
        Response.Cookies.Delete(DefaultCookies.Session);

        return Ok(HttpResponseBuilder.Create().Succeeded().Build());
    }
}