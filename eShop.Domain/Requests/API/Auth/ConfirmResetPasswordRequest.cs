namespace eShop.Domain.Requests.API.Auth;

public record ConfirmResetPasswordRequest
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}