namespace eShop.Domain.DTOs;

public class UserProviderDto
{
    public required TwoFactorMethod Method { get; set; }
    public required bool IsPrimary { get; set; }
    public string? Credential { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}