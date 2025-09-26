using System.Security.Claims;
using eShop.Blazor.Server.Domain.DTOs;

namespace eShop.Blazor.Server.Domain.Types;

public class SignInResponse
{
    public required string AccessToken { get; set; }
    public required ClaimsIdentityDto Identity { get; set; }
}