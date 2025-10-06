using eShop.Blazor.Server.Domain.DTOs;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Domain.Types;
using eShop.Blazor.Server.Infrastructure.Security;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.API.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Blazor.Server.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    TokenProvider tokenProvider,
    ISecurityService securityService) : ControllerBase
{
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly ISecurityService securityService = securityService;

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest signInRequest)
    {
        var request = new AuthorizeRequest() { UserId = signInRequest.UserId };
        var result = await securityService.AuthorizeAsync(request);
        if (!result.Success) return StatusCode(500, result);

        var response = result.Get<AuthorizeResponse>()!;
        tokenProvider.AccessToken = response.AccessToken;

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

        var signInResponse = new SignInResponse()
        {
            Identity = claimsIdentity,
            AccessToken = tokenProvider.AccessToken
        };

        return Ok(new ResponseBuilder().Succeeded().WithResult(signInResponse).Build());
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync([FromBody] SignOutRequest signOutRequest)
    {
        tokenProvider.AccessToken = signOutRequest.AccessToken;
        var request = new UnauthorizeRequest(){ UserId = signOutRequest.UserId };
        var result = await securityService.UnauthorizeAsync(request);
        if (!result.Success) return StatusCode(500, result);
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new ResponseBuilder().Succeeded().Build());
    }
}