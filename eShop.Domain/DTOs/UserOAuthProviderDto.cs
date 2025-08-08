namespace eShop.Domain.DTOs;

public class UserOAuthProviderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAllowed { get; set; }
    public DateTimeOffset? LinkedDate { get; set; }
    public DateTimeOffset? DisallowedDate { get; set; }
}