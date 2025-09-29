namespace eShop.Domain.DTOs;

public class UserLoginMethodsDto
{
    public bool HasPassword { get; set; }
    public bool HasTwoFactor { get; set; }
    public bool HasLinkedAccounts { get; set; }
    public bool HasPasskeys { get; set; }
}