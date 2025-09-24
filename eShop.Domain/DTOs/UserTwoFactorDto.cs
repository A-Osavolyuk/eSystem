namespace eShop.Domain.DTOs;

public class UserTwoFactorDto
{
    public bool TwoFactorEnabled  { get; set; }
    public List<UserProviderDto> Providers { get; set; } = [];
}