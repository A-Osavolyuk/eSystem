namespace eShop.Domain.Responses.API.Auth;

public class ConfirmChangePhoneNumberResponse
{
    public string Message { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}