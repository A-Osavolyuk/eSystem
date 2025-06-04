namespace eShop.Domain.Types;

public class UserStore
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? AvatarUri { get; set; } = string.Empty;
}