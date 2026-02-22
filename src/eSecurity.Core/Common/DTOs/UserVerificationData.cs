using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Common.DTOs;

public class UserVerificationData
{
    public VerificationMethod PreferredMethod { get; set; }
    public bool EmailEnabled { get; set; }
    public bool AuthenticatorEnabled { get; set; }
    public bool PasskeyEnabled { get; set; }
}