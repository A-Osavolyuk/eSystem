namespace eShop.Domain.Responses.Auth;

public class ChangeTwoFactorStateResponse
{
    public string Message { get; set; } = string.Empty;
    public bool TwoFactorAuthenticationState { get; set; } = false;
}