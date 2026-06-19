namespace eSecurity.Idp.Security.Identity.Options;

public class AccountOptions
{
    public uint PrimaryEmailMaxCount { get; set; } = 1;
    public uint RecoveryEmailMaxCount { get; set; } = 1;
    public uint SecondaryEmailMaxCount { get; set; } = 3;

    public uint PrimaryPhoneNumberMaxCount { get; set; } = 1;
    public uint RecoveryPhoneNumberMaxCount { get; set; } = 1;
    public uint SecondaryPhoneNumberMaxCount { get; set; } = 3;
}