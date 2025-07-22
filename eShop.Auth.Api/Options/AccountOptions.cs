namespace eShop.Auth.Api.Options;

public class AccountOptions
{
    public bool RequireConfirmedEmail { get; set; } = true;
    public bool RequireConfirmedRecoveryEmail { get; set; } = false;
    public bool RequireConfirmedPhoneNumber { get; set; } = false;
    public bool RequireConfirmedAccount { get; set; } = true;
    
    public bool RequireUniqueEmail { get; set; } = true;
    public bool RequireUniqueRecoveryEmail { get; set; } = true;
    public bool RequireUniqueUserName { get; set; } = true;
    public bool RequireUniquePhoneNumber { get; set; } = true;
}