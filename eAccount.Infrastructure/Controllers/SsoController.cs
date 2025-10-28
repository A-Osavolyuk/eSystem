using eAccount.Infrastructure.Security.Authentication.JWT;
using eAccount.Infrastructure.Security.Authentication.SSO;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.Cookies;
using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eAccount.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SsoController(
    TokenProvider tokenProvider,
    ISsoService ssoService,
    IProtectorFactory factory) : ControllerBase
{
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly ISsoService ssoService = ssoService;
    private readonly IProtectorFactory factory = factory;

    [HttpPost("authorize")]
    public async Task<IActionResult> AuthorizeAsync([FromBody] AuthorizeRequest request)
    {
        var result = await ssoService.AuthorizeAsync(request);
        if (!result.Success) return StatusCode(500, result);

        var response = result.Get<AuthorizeResponse>()!;

        if (!request.State.Equals(response.State))
        {
            return BadRequest(
                new ResponseBuilder()
                    .Failed()
                    .WithMessage("Invalid state.")
                    .Build());
        }

        var cookie = new SessionCookie()
        {
            SessionId = response.SessionId,
            UserId = request.UserId,
            DeviceId = response.DeviceId,
            IssuedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(30)
        };

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

        return Ok(result);
    }
}