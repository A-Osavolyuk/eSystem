using eSystem.Domain.Security.Verification;

namespace eSystem.Domain.DTOs;

public class UserVerificationData
{
    public VerificationMethod PreferredMethod { get; set; }
    public bool EmailEnabled { get; set; }
    public bool AuthenticatorEnabled { get; set; }
    public bool PasskeyEnabled { get; set; }
    public List<UserVerificationMethod> Methods { get; set; } = [];
}