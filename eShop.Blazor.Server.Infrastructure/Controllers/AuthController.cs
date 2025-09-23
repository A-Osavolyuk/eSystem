using eShop.Blazor.Server.Domain.DTOs;
using eShop.Blazor.Server.Domain.Types;
using eShop.Blazor.Server.Infrastructure.Security;
using eShop.Domain.Common.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Blazor.Server.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenProvider tokenProvider) : ControllerBase
{
    private readonly TokenProvider tokenProvider = tokenProvider;

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        if (string.IsNullOrEmpty(request.AccessToken))
        {
            return BadRequest(new ResponseBuilder()
                    .Failed()
                    .WithMessage("Invalid access token.")
                    .Build());
        }
        
        tokenProvider.AccessToken = request.AccessToken;

        var result = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            return Unauthorized(new ResponseBuilder()
                .Failed()
                .Build());
        }

        var principal = result.Principal!;
        var properties = new AuthenticationProperties() { IsPersistent = true, };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

        var claims = principal.Claims.Select(c => new ClaimDto
        {
            Type = c.Type, 
            Value = c.Value
        }).ToList();
        
        var claimsIdentity = new ClaimIdentityDto
        {
            Claims = claims, 
            Scheme = CookieAuthenticationDefaults.AuthenticationScheme
        };
        
        return Ok(new ResponseBuilder().Succeeded().WithResult(claimsIdentity).Build());
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync()
    {
        await  HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new ResponseBuilder().Succeeded().Build());
    }
}