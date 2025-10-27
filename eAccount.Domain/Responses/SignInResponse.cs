using eAccount.Domain.DTOs;

namespace eAccount.Domain.Responses;

public class SignInResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public Identity Identity { get; set; } = null!;
}