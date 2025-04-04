namespace eShop.Domain.Responses.Api.Admin;

public class LockoutUserResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}