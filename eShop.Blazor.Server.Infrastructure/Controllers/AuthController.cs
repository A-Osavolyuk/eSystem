using eShop.Blazor.Server.Domain.Types;
using eShop.Blazor.Server.Infrastructure.Security;
using eShop.Domain.Common.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Blazor.Server.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    TokenProvider tokenProvider,
    AuthenticationStateProvider authenticationStateProvider) : ControllerBase
{
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] AuthenticationRequest request)
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

        await (authenticationStateProvider as JwtAuthenticationStateProvider)!.SignInAsync(result.Principal, HttpContext);
        return Ok(new ResponseBuilder().Succeeded().Build());
    }
}