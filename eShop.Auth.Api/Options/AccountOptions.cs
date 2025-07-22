namespace eShop.Auth.Api.Options;

public class AccountOptions
{
    public bool RequireUniqueEmail { get; set; } = true;
    public bool RequireUniqueRecoveryEmail { get; set; } = true;
    public bool RequireUniqueUserName { get; set; } = true;
    public bool RequireUniquePhoneNumber { get; set; } = true;
}