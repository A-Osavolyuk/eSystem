using eShop.Blazor.Server.Domain.DTOs;

namespace eShop.Blazor.Server.Domain.Responses;

public class SignInResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public ClaimsIdentityDto Identity { get; set; } = null!;
}