using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCinema.Server.Controllers.v1;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthenticationController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("login")]
    [EndpointSummary("Login")]
    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties()
        {
            IsPersistent = true,
            RedirectUri = "https://localhost:6511/home"
        });
    }
}