using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace eSystem.Auth.Api.Security.Authentication.Results;

public class AuthenticationResult
{
    public ClaimsPrincipal Principal { get; set; } = new();
    public AuthenticationProperties Properties { get; set; } = new();
}