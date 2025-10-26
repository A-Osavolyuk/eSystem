using eSystem.Core.Security.Verification;

namespace eSystem.Core.DTOs;

public class UserVerificationData
{
    public VerificationMethod PreferredMethod { get; set; }
    public bool EmailEnabled { get; set; }
    public bool AuthenticatorEnabled { get; set; }
    public bool PasskeyEnabled { get; set; }
    public List<UserVerificationMethod> Methods { get; set; } = [];
}