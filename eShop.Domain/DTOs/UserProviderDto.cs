namespace eShop.Domain.DTOs;

public class UserProviderDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string? Credential { get; set; }
    public required bool Subscribed { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}