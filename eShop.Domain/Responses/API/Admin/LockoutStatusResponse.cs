namespace eShop.Domain.Responses.Api.Admin;

public class LockoutStatusResponse
{
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}