using eAccount.Domain.DTOs;
using eAccount.Domain.Requests;
using eAccount.Infrastructure.Security;
using eAccount.Infrastructure.Security.Authentication.SSO;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.Cookies;
using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInRequest = eAccount.Domain.Requests.SignInRequest;
using SignInResponse = eAccount.Domain.Responses.SignInResponse;

namespace eAccount.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    TokenProvider tokenProvider,
    ISsoService ssoService,
    IProtector protector) : ControllerBase
{
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly ISsoService ssoService = ssoService;
    private readonly IProtector protector = protector;

    [HttpPost("authorize")]
    public async Task<IActionResult> AuthorizeAsync([FromBody] AuthorizeRequest request)
    {
        var result = await ssoService.AuthorizeAsync(request);
        if (!result.Success) return StatusCode(500, result);
        
        var response = result.Get<AuthorizeResponse>()!;
        var sessionCookie = new SessionCookie()
        {
            SessionId = response.SessionId,
            UserId = request.UserId,
            DeviceId = response.DeviceId,
            Nonce = response.Nonce,
            IssuedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(30)
        };
        
        var sessionCookieJson = JsonSerializer.Serialize(sessionCookie);
        var protectedSessionCookie = protector.Protect(sessionCookieJson);
        var cookieOptions = new CookieOptions()
        {
            Domain = "localhost",
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = sessionCookie.ExpiresAt.UtcDateTime,
            MaxAge = TimeSpan.FromDays(30)
        };
        
        Response.Cookies.Append(DefaultCookies.Session, protectedSessionCookie, cookieOptions);
    
        return Ok(new ResponseBuilder().Succeeded().Build());
    }
    
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
        };

        Response.Cookies.Append(DefaultCookies.RefreshToken, request.RefreshToken, cookieOptions);
        tokenProvider.AccessToken = request.AccessToken;

        var authenticateResult = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return Unauthorized(new ResponseBuilder()
                .Failed()
                .Build());
        }

        var principal = authenticateResult.Principal!;
        var properties = new AuthenticationProperties() { IsPersistent = true, };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

        var claims = principal.Claims.Select(c => new ClaimDto
        {
            Type = c.Type,
            Value = c.Value
        }).ToList();

        var claimsIdentity = new ClaimsIdentityDto
        {
            Claims = claims,
            Scheme = CookieAuthenticationDefaults.AuthenticationScheme
        };

        var response = new SignInResponse()
        {
            Identity = claimsIdentity,
            AccessToken = tokenProvider.AccessToken
        };

        return Ok(new ResponseBuilder().Succeeded().WithResult(response).Build());
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync([FromBody] SignOutRequest signOutRequest)
    {
        tokenProvider.AccessToken = signOutRequest.AccessToken;

        var refreshToken = Request.Cookies[DefaultCookies.RefreshToken]!;
        var request = new UnauthorizeRequest()
        {
            UserId = signOutRequest.UserId,
            RefreshToken = refreshToken
        };
        
        var result = await ssoService.UnauthorizeAsync(request);
        if (!result.Success) return StatusCode(500, result);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(DefaultCookies.RefreshToken);
        
        return Ok(new ResponseBuilder().Succeeded().Build());
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var refreshToken = Request.Cookies[DefaultCookies.RefreshToken];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new ResponseBuilder()
                .Failed()
                .Build());
        }
        
        request.RefreshToken = refreshToken;
        var result = await ssoService.RefreshTokenAsync(request);

        return Ok(result);
    }
}