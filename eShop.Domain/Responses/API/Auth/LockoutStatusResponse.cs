namespace eShop.Domain.Responses.API.Auth;

public class LockoutStatusResponse
{
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}