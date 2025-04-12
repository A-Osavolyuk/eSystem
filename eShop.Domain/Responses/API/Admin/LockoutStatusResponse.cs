namespace eShop.Domain.Responses.API.Admin;

public class LockoutStatusResponse
{
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}