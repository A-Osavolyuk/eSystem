using System.Security.Claims;
using eShop.Blazor.Server.Domain.DTOs;

namespace eShop.Blazor.Server.Domain.Types;

public class SignInResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public ClaimsIdentityDto Identity { get; set; } = null!;
}