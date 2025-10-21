namespace eShop.Auth.Api.Security.Identity;

public class AccountOptions
{
    public bool RequireUniqueEmail { get; set; } = true;
    public bool RequireUniqueRecoveryEmail { get; set; } = true;
    public bool RequireUniqueUserName { get; set; } = true;
    public bool RequireUniquePhoneNumber { get; set; } = true;

    public uint PrimaryEmailMaxCount { get; set; } = 1;
    public uint RecoveryEmailMaxCount { get; set; } = 1;
    public uint SecondaryEmailMaxCount { get; set; } = 3;

    public uint PrimaryPhoneNumberMaxCount { get; set; } = 1;
    public uint RecoveryPhoneNumberMaxCount { get; set; } = 1;
    public uint SecondaryPhoneNumberMaxCount { get; set; } = 3;
}