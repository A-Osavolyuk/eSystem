using eAccount.Domain.DTOs;

namespace eAccount.Domain.Responses;

public class SignInResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public ClaimsIdentityDto Identity { get; set; } = null!;
}