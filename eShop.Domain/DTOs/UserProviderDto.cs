namespace eShop.Domain.DTOs;

public class UserProviderDto
{
    public required Guid Id { get; set; }
    public required MethodType Type { get; set; }
    public required bool IsPrimary { get; set; }
    public string? Credential { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}